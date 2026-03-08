using XafMaui.Data;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class ProjectsPage : ContentPage
{
    public ProjectsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ProjectsViewModel)BindingContext).LoadProjects();
    }

    async void OnProjectTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is LocalProject project)
            await Shell.Current.GoToAsync($"{nameof(ProjectDetailPage)}?id={project.ID}");
    }
}
