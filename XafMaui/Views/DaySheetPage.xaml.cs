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
}
