using XafMaui.Data;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class ClientsPage : ContentPage
{
    public ClientsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ClientsViewModel)BindingContext).LoadClients();
    }

    async void OnClientTapped(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        if (e.Item is LocalClient client)
            await Shell.Current.GoToAsync($"{nameof(ClientDetailPage)}?id={client.ID}");
    }
}
