using DevExpress.Maui.CollectionView;
using XafMaui.Models;
using XafMaui.Services;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class ReportsPage : ContentPage
{
    ReportsViewModel ViewModel => (ReportsViewModel)BindingContext;

    public ReportsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadAll();
    }

    async void OnDownloadTimesheetClicked(object sender, EventArgs e)
    {
        var weekStart = ViewModel.TimesheetWeekStart.ToString("yyyy-MM-dd");
        await DownloadAndOpenPdf($"ProjectReport/WeeklyTimesheet?weekStart={weekStart}", "WeeklyTimesheet.pdf");
    }

    async void OnDownloadBudgetClicked(object sender, EventArgs e)
    {
        var endpoint = "ProjectReport/ProjectBudget";
        if (ViewModel.BudgetProject != null)
            endpoint += $"?projectId={ViewModel.BudgetProject.ID}";
        await DownloadAndOpenPdf(endpoint, "ProjectBudgetReport.pdf");
    }

    async void OnLoadReportsClicked(object sender, EventArgs e)
    {
        try
        {
            pdfSpinner.IsVisible = true;
            pdfSpinner.IsRunning = true;
            await ViewModel.LoadStoredReportsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to load reports: {ex.Message}", "OK");
        }
        finally
        {
            pdfSpinner.IsRunning = false;
            pdfSpinner.IsVisible = false;
        }
    }

    async void OnReportTapped(object sender, CollectionViewGestureEventArgs e)
    {
        if (e.Item is not ReportItemDto report)
            return;

        await DownloadAndOpenPdf($"Report/DownloadByKey({report.ID})", $"{report.DisplayName}.pdf");
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
