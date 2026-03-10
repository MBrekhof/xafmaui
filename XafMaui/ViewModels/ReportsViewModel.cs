using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.ViewModels;

public class DayHours
{
    public string Day { get; set; } = string.Empty;
    public decimal Hours { get; set; }
}

public class TaskHours
{
    public string TaskName { get; set; } = string.Empty;
    public decimal Estimated { get; set; }
    public decimal Actual { get; set; }
}

public class ReportsViewModel : INotifyPropertyChanged
{
    LocalProject? _selectedProject;
    LocalProject? _budgetProject;
    bool _isRefreshing;
    DateTime _timesheetWeekStart;

    public ReportsViewModel()
    {
        // Default to Monday of current week
        var today = DateTime.Today;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        _timesheetWeekStart = today.AddDays(-diff);
    }

    public ObservableCollection<DayHours> WeeklyHours { get; } = [];
    public ObservableCollection<TaskHours> TaskBreakdown { get; } = [];
    public ObservableCollection<LocalProject> Projects { get; } = [];
    public ObservableCollection<ReportItemDto> StoredReports { get; } = [];

    public LocalProject? SelectedProject
    {
        get => _selectedProject;
        set { _selectedProject = value; OnPropertyChanged(); LoadProjectReport(); }
    }

    public LocalProject? BudgetProject
    {
        get => _budgetProject;
        set { _budgetProject = value; OnPropertyChanged(); }
    }

    public DateTime TimesheetWeekStart
    {
        get => _timesheetWeekStart;
        set { _timesheetWeekStart = value; OnPropertyChanged(); }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }

    public ICommand RefreshCommand => new Command(async () =>
    {
        var sync = IPlatformApplication.Current!.Services.GetRequiredService<Services.SyncService>();
        await sync.SyncAllAsync();
        LoadAll();
        IsRefreshing = false;
    });

    public double BudgetBurnPercent { get; private set; }

    public void LoadAll()
    {
        LoadWeeklyHours();
        LoadProjects();
    }

    void LoadWeeklyHours()
    {
        WeeklyHours.Clear();
        using var db = new LocalDbContext();

        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday) startOfWeek = startOfWeek.AddDays(-7);

        for (int i = 0; i < 7; i++)
        {
            var date = startOfWeek.AddDays(i);
            var hours = db.TimeEntries.Where(t => t.Date.Date == date).Sum(t => (decimal?)t.Hours) ?? 0;
            WeeklyHours.Add(new DayHours
            {
                Day = date.ToString("ddd"),
                Hours = hours
            });
        }
    }

    void LoadProjects()
    {
        Projects.Clear();
        using var db = new LocalDbContext();
        foreach (var p in db.Projects.OrderBy(p => p.Name))
            Projects.Add(p);
    }

    void LoadProjectReport()
    {
        TaskBreakdown.Clear();
        if (_selectedProject == null) return;

        using var db = new LocalDbContext();
        var tasks = db.ProjectTasks.Where(t => t.ProjectID == _selectedProject.ID).OrderBy(t => t.SortOrder).ToList();
        var entries = db.TimeEntries.Where(e => tasks.Select(t => t.ID).Contains(e.ProjectTaskID)).ToList();

        decimal totalLogged = 0;
        foreach (var task in tasks)
        {
            var actual = entries.Where(e => e.ProjectTaskID == task.ID).Sum(e => e.Hours);
            totalLogged += actual;
            TaskBreakdown.Add(new TaskHours
            {
                TaskName = task.Name,
                Estimated = task.EstimatedHours,
                Actual = actual
            });
        }

        BudgetBurnPercent = _selectedProject.BudgetHours > 0
            ? (double)(totalLogged / _selectedProject.BudgetHours) * 100
            : 0;
        OnPropertyChanged(nameof(BudgetBurnPercent));
    }

    public async Task LoadStoredReportsAsync()
    {
        StoredReports.Clear();
        var api = IPlatformApplication.Current!.Services.GetRequiredService<Services.ApiClient>();
        var reports = await api.GetListAsync<ReportItemDto>("ReportDataV2", "$select=ID,DisplayName,DataTypeName&$orderby=DisplayName");
        foreach (var r in reports)
            StoredReports.Add(r);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
