using XafMaui.Services;

namespace XafMaui.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ClientDetailPage), typeof(ClientDetailPage));
        Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
        Routing.RegisterRoute(nameof(AddTimeEntryPage), typeof(AddTimeEntryPage));
        Routing.RegisterRoute(nameof(EditTimeEntryPage), typeof(EditTimeEntryPage));
    }

    async void OnLogoutClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Logout", "Are you sure you want to log out?", "Yes", "Cancel");
        if (!confirm) return;

        var auth = IPlatformApplication.Current!.Services.GetRequiredService<AuthService>();
        var sync = IPlatformApplication.Current!.Services.GetRequiredService<SyncService>();
        auth.Logout();
        await sync.ClearLocalDataAsync();
        Application.Current!.Windows[0].Page = IPlatformApplication.Current!.Services.GetRequiredService<LoginPage>();
    }
}
