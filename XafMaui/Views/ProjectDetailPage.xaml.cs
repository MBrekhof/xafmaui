using XafMaui.Data;

namespace XafMaui.Views;

[QueryProperty(nameof(ProjectId), "id")]
public partial class ProjectDetailPage : ContentPage
{
    public int ProjectId { get; set; }

    public ProjectDetailPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadProject();
    }

    void LoadProject()
    {
        using var db = new LocalDbContext();
        var project = db.Projects.FirstOrDefault(p => p.ID == ProjectId);
        if (project == null) return;

        projectName.Text = project.Name;
        projectDescription.Text = project.Description;

        var tasks = db.ProjectTasks.Where(t => t.ProjectID == ProjectId).OrderBy(t => t.SortOrder).ToList();
        var timeEntries = db.TimeEntries.Where(t => tasks.Select(tk => tk.ID).Contains(t.ProjectTaskID)).ToList();

        var totalEstimated = tasks.Sum(t => t.EstimatedHours);
        var totalLogged = timeEntries.Sum(t => t.Hours);

        budgetLabel.Text = $"Budget: {project.BudgetHours:F0}h | Estimated: {totalEstimated:F0}h | Logged: {totalLogged:F1}h";
        var progress = totalEstimated > 0 ? (double)(totalLogged / totalEstimated) : 0;
        progressBar.Progress = Math.Min(progress, 1.0);
        progressLabel.Text = $"{progress:P0} complete";

        var taskViewModels = tasks.Select(t =>
        {
            var logged = timeEntries.Where(e => e.ProjectTaskID == t.ID).Sum(e => e.Hours);
            return new TaskViewModel
            {
                Name = t.Name,
                EstimatedHours = t.EstimatedHours,
                LoggedHours = logged,
            };
        }).ToList();

        tasksView.ItemsSource = taskViewModels;
    }
}

public class TaskViewModel
{
    public string Name { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public decimal LoggedHours { get; set; }
    public string HoursDisplay => $"{LoggedHours:F1} / {EstimatedHours:F0}h";
    public double ProgressPercent => EstimatedHours > 0 ? Math.Min((double)(LoggedHours / EstimatedHours), 1.0) : 0;
}
