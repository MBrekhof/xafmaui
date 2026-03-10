using XafMaui.Views;

namespace XafMaui
{
    public partial class App : Application
    {
        readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;

            var isDark = Preferences.Get("ThemeIsDark", true);
            UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_services.GetRequiredService<LoginPage>());
        }
    }
}
