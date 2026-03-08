using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.Views;

[QueryProperty(nameof(DateString), "date")]
public partial class AddTimeEntryPage : ContentPage
{
    public string DateString { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    List<LocalProject> _projects = [];
    List<LocalProjectTask> _tasks = [];

    public AddTimeEntryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        using var db = new LocalDbContext();
        _projects = db.Projects.Where(p => p.Status == (int)ProjectStatus.Active).OrderBy(p => p.Name).ToList();
        projectPicker.ItemsSource = _projects;
    }

    void OnProjectSelected(object sender, EventArgs e)
    {
        if (projectPicker.SelectedItem is LocalProject project)
        {
            using var db = new LocalDbContext();
            _tasks = db.ProjectTasks
                .Where(t => t.ProjectID == project.ID)
                .OrderBy(t => t.SortOrder)
                .ToList();
            taskPicker.ItemsSource = _tasks;
        }
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (taskPicker.SelectedItem is not LocalProjectTask task)
        {
            await DisplayAlertAsync("Required", "Please select a project and task.", "OK");
            return;
        }

        var project = projectPicker.SelectedItem as LocalProject;
        var date = DateTime.Parse(DateString);

        using var db = new LocalDbContext();
        db.TimeEntries.Add(new LocalTimeEntry
        {
            Date = date,
            Hours = (decimal)(hoursEdit.Value ?? 1),
            Note = noteEdit.Text,
            Status = (int)TimeEntryStatus.Draft,
            ProjectTaskID = task.ID,
            ProjectTaskName = task.Name,
            ProjectName = project?.Name,
            IsPendingSync = true,
        });
        await db.SaveChangesAsync();

        await Shell.Current.GoToAsync("..");
    }
}
