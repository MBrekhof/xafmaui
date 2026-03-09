using DevExpress.Maui;
using DevExpress.Maui.Core;
using SkiaSharp.Views.Maui.Controls.Hosting;
using XafMaui.Services;
using XafMaui.Views;

namespace XafMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            ThemeManager.ApplyThemeToSystemBars = true;
            ThemeManager.UseAndroidSystemColor = false;
            ThemeManager.Theme = new Theme(ThemeSeedColor.TealGreen);
            var builder = MauiApp.CreateBuilder()
                .UseMauiApp<App>()
                .UseDevExpress(useLocalization: false)
                .UseDevExpressControls()
                .UseDevExpressCharts()
                .UseDevExpressTreeView()
                .UseDevExpressCollectionView()
                .UseDevExpressEditors()
                .UseDevExpressDataGrid()
                .UseDevExpressScheduler()
                .UseDevExpressGauges()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                });

            // HTTP client (dev cert validation disabled in debug)
            builder.Services.AddSingleton(_ =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif
                return new HttpClient(handler);
            });

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiClient>();
            builder.Services.AddSingleton<SyncService>();
            builder.Services.AddTransient<LoginPage>();

            return builder.Build();
        }
    }
}
