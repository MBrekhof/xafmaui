using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.ViewModels;

public class ProjectGroup : List<LocalProject>
{
    public string StatusName { get; }
    public ProjectGroup(string statusName, IEnumerable<LocalProject> projects) : base(projects)
    {
        StatusName = statusName;
    }
}

public class ProjectsViewModel : INotifyPropertyChanged
{
    bool _isRefreshing;

    public ObservableCollection<ProjectGroup> ProjectGroups { get; } = [];

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }

    public ICommand RefreshCommand => new Command(async () =>
    {
        var sync = IPlatformApplication.Current!.Services.GetRequiredService<Services.SyncService>();
        await sync.SyncProjectsAsync();
        LoadProjects();
        IsRefreshing = false;
    });

    public void LoadProjects()
    {
        ProjectGroups.Clear();
        using var db = new LocalDbContext();
        var projects = db.Projects.OrderBy(p => p.Name).ToList();

        var statusNames = new[] { "Draft", "Active", "On Hold", "Completed", "Archived" };
        for (int i = 0; i < statusNames.Length; i++)
        {
            var group = projects.Where(p => p.Status == i).ToList();
            if (group.Count > 0)
                ProjectGroups.Add(new ProjectGroup(statusNames[i], group));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
