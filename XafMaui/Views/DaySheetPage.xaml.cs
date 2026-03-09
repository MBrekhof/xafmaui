using DevExpress.Maui.Controls;
using DevExpress.Maui.DataGrid;
using XafMaui.Data;
using XafMaui.Models;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class DaySheetPage : ContentPage
{
    DaySheetViewModel ViewModel => (DaySheetViewModel)BindingContext;

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

        sheetProjectLabel.Text = entry.ProjectName ?? "—";
        sheetTaskLabel.Text = entry.ProjectTaskName ?? "—";
        sheetHoursLabel.Text = entry.Hours.ToString("F1");
        sheetStatusLabel.Text = ((TimeEntryStatus)entry.Status).ToString();
        sheetNoteLabel.Text = string.IsNullOrWhiteSpace(entry.Note) ? "(no notes)" : entry.Note;

        detailSheet.State = BottomSheetState.HalfExpanded;
    }
}
