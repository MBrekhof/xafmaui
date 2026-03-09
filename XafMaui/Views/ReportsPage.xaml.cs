using XafMaui.Services;
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

    async void OnDownloadTimesheetClicked(object sender, EventArgs e)
    {
        await DownloadAndOpenPdf("ProjectReport/WeeklyTimesheet", "WeeklyTimesheet.pdf");
    }

    async void OnDownloadBudgetClicked(object sender, EventArgs e)
    {
        await DownloadAndOpenPdf("ProjectReport/ProjectBudget", "ProjectBudgetReport.pdf");
    }

    async Task DownloadAndOpenPdf(string endpoint, string filename)
    {
        try
        {
            pdfSpinner.IsVisible = true;
            pdfSpinner.IsRunning = true;

            var api = IPlatformApplication.Current!.Services.GetRequiredService<ApiClient>();
            var pdfBytes = await api.GetBytesAsync(endpoint);

            var filePath = Path.Combine(FileSystem.CacheDirectory, filename);
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath, "application/pdf")
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to download report: {ex.Message}", "OK");
        }
        finally
        {
            pdfSpinner.IsRunning = false;
            pdfSpinner.IsVisible = false;
        }
    }
}
