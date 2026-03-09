using XafMaui.Services;

namespace XafMaui.Views;

public partial class LoginPage : ContentPage
{
    readonly AuthService _auth;
    readonly SyncService _sync;

    public LoginPage(AuthService auth, SyncService sync)
    {
        InitializeComponent();
        _auth = auth;
        _sync = sync;
    }

    async void OnLoginClicked(object sender, EventArgs e)
    {
        userNameEdit.HasError = false;
        loginButton.IsEnabled = false;

        try
        {
            var token = await _auth.LoginAsync(
                userNameEdit.Text ?? "",
                passwordEdit.Text ?? "");

            if (token != null)
            {
                await _sync.SyncAllAsync();
                Application.Current!.Windows[0].Page = new AppShell();
            }
            else
            {
                userNameEdit.ErrorText = "Invalid user name or password";
                userNameEdit.HasError = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Connection Error", ex.Message, "OK");
        }

        loginButton.IsEnabled = true;
    }
}
