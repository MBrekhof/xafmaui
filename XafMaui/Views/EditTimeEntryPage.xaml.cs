using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.Views;

[QueryProperty(nameof(LocalIdString), "localId")]
public partial class EditTimeEntryPage : ContentPage
{
    public string LocalIdString { get; set; } = "0";

    public EditTimeEntryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!int.TryParse(LocalIdString, out var localId)) return;

        using var db = new LocalDbContext();
        var entry = db.TimeEntries.Find(localId);
        if (entry == null) return;

        projectLabel.Text = entry.ProjectName ?? "—";
        taskLabel.Text = entry.ProjectTaskName ?? "—";
        hoursEdit.Value = entry.Hours;
        noteEdit.Text = entry.Note;

        if (!string.IsNullOrWhiteSpace(entry.ReviewNote))
        {
            rejectionBanner.IsVisible = true;
            rejectionNoteLabel.Text = entry.ReviewNote;
        }
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(LocalIdString, out var localId)) return;

        using var db = new LocalDbContext();
        var entry = db.TimeEntries.Find(localId);
        if (entry == null) return;

        entry.Hours = (decimal)(hoursEdit.Value ?? 1);
        entry.Note = noteEdit.Text;
        entry.Status = (int)TimeEntryStatus.Draft;
        entry.ReviewNote = null;
        entry.IsPendingSync = true;
        await db.SaveChangesAsync();

        await Shell.Current.GoToAsync("..");
    }
}
