using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ReportsViewModel)BindingContext).LoadAll();
    }
}
