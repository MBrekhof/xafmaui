using DevExpress.Maui.Controls;
using DevExpress.Maui.DataGrid;
using XafMaui.Data;
using XafMaui.Models;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class DaySheetPage : ContentPage
{
    DaySheetViewModel ViewModel => (DaySheetViewModel)BindingContext;
    LocalTimeEntry? _selectedEntry;

    public DaySheetPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadEntries();
    }

    async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AddTimeEntryPage)}?date={ViewModel.SelectedDate:yyyy-MM-dd}");
    }

    void OnSubmitClicked(object sender, EventArgs e)
    {
        ViewModel.SubmitDraftEntries();
    }

    void OnGridTap(object sender, DataGridGestureEventArgs e)
    {
        if (e.Item is not LocalTimeEntry entry)
            return;

        _selectedEntry = entry;

        sheetProjectLabel.Text = entry.ProjectName ?? "—";
        sheetTaskLabel.Text = entry.ProjectTaskName ?? "—";
        sheetHoursLabel.Text = entry.Hours.ToString("F1");
        var status = (TimeEntryStatus)entry.Status;
        sheetStatusLabel.Text = status.ToString();
        sheetStatusLabel.TextColor = status switch
        {
            TimeEntryStatus.Draft => Colors.Gray,
            TimeEntryStatus.Submitted => Color.FromArgb("#FFA726"),
            TimeEntryStatus.Approved => Color.FromArgb("#66BB6A"),
            TimeEntryStatus.Rejected => Color.FromArgb("#EF5350"),
            _ => Colors.Gray,
        };
        sheetNoteLabel.Text = string.IsNullOrWhiteSpace(entry.Note) ? "(no notes)" : entry.Note;

        // Show review note section for rejected entries
        bool isRejected = status == TimeEntryStatus.Rejected;
        reviewNoteSection.IsVisible = isRejected;
        if (isRejected)
            sheetReviewNoteLabel.Text = string.IsNullOrWhiteSpace(entry.ReviewNote) ? "(no rejection note)" : entry.ReviewNote;

        detailSheet.State = BottomSheetState.HalfExpanded;
    }

    async void OnEditRejectedClicked(object sender, EventArgs e)
    {
        if (_selectedEntry == null) return;
        detailSheet.State = BottomSheetState.Hidden;
        await Shell.Current.GoToAsync($"{nameof(EditTimeEntryPage)}?localId={_selectedEntry.LocalId}");
    }
}
