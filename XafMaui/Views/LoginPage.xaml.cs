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
        errorLabel.IsVisible = false;
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
                errorLabel.Text = "Invalid credentials";
                errorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            errorLabel.Text = $"Connection error: {ex.Message}";
            errorLabel.IsVisible = true;
        }

        loginButton.IsEnabled = true;
    }
}
