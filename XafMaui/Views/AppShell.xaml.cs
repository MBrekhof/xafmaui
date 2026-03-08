namespace XafMaui.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ClientDetailPage), typeof(ClientDetailPage));
        Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
        Routing.RegisterRoute(nameof(AddTimeEntryPage), typeof(AddTimeEntryPage));
    }
}
