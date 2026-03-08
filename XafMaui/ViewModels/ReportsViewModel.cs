using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XafMaui.Data;

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

    public ObservableCollection<DayHours> WeeklyHours { get; } = [];
    public ObservableCollection<TaskHours> TaskBreakdown { get; } = [];
    public ObservableCollection<LocalProject> Projects { get; } = [];

    public LocalProject? SelectedProject
    {
        get => _selectedProject;
        set { _selectedProject = value; OnPropertyChanged(); LoadProjectReport(); }
    }

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

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
