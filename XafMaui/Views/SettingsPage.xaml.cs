using DevExpress.Maui.Core;

namespace XafMaui.Views;

public class ThemeOption
{
    public string Name { get; set; } = "";
    public ThemeSeedColor SeedColor { get; set; }
    public string HexColor { get; set; } = "";
}

public partial class SettingsPage : ContentPage
{
    static readonly List<ThemeOption> ThemeOptions =
    [
        new() { Name = "Purple", SeedColor = ThemeSeedColor.Purple, HexColor = "#65558F" },
        new() { Name = "Red", SeedColor = ThemeSeedColor.Red, HexColor = "#BA182B" },
        new() { Name = "Green", SeedColor = ThemeSeedColor.Green, HexColor = "#356B00" },
        new() { Name = "Dark Cyan", SeedColor = ThemeSeedColor.DarkCyan, HexColor = "#00677E" },
        new() { Name = "Teal Green", SeedColor = ThemeSeedColor.TealGreen, HexColor = "#006C50" },
        new() { Name = "Deep Sea Blue", SeedColor = ThemeSeedColor.DeepSeaBlue, HexColor = "#006590" },
        new() { Name = "Dark Green", SeedColor = ThemeSeedColor.DarkGreen, HexColor = "#006D41" },
        new() { Name = "Violet", SeedColor = ThemeSeedColor.Violet, HexColor = "#9B2D9B" },
        new() { Name = "Brown", SeedColor = ThemeSeedColor.Brown, HexColor = "#765B00" },
        new() { Name = "Blue", SeedColor = ThemeSeedColor.Blue, HexColor = "#0057CE" },
    ];

    public SettingsPage()
    {
        InitializeComponent();
        themePicker.ItemsSource = ThemeOptions;

        var saved = Preferences.Get("ThemeColor", "TealGreen");
        var current = ThemeOptions.FirstOrDefault(t => t.SeedColor.ToString() == saved)
                      ?? ThemeOptions[4]; // TealGreen default
        themePicker.SelectedItem = current;
        UpdatePreview(current);
        darkModeSwitch.IsToggled = Preferences.Get("ThemeIsDark", true);
    }

    void OnThemeChanged(object sender, EventArgs e)
    {
        if (themePicker.SelectedItem is not ThemeOption selected)
            return;

        Preferences.Set("ThemeColor", selected.SeedColor.ToString());
        UpdatePreview(selected);
        restartHint.IsVisible = true;

        // Apply immediately for DevExpress controls
        ThemeManager.Theme = new Theme(selected.SeedColor);
    }

    void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Set("ThemeIsDark", e.Value);
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    void UpdatePreview(ThemeOption option)
    {
        previewGrid.BackgroundColor = Color.FromArgb(option.HexColor);
    }
}
