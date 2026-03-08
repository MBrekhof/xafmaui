using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XafMaui.Data;

namespace XafMaui.ViewModels;

public class ClientsViewModel : INotifyPropertyChanged
{
    string _searchText = string.Empty;
    List<LocalClient> _allClients = [];

    public ObservableCollection<LocalClient> Clients { get; } = [];

    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); FilterClients(); }
    }

    public void LoadClients()
    {
        using var db = new LocalDbContext();
        _allClients = db.Clients.OrderBy(c => c.Name).ToList();
        FilterClients();
    }

    void FilterClients()
    {
        Clients.Clear();
        var filtered = string.IsNullOrWhiteSpace(_searchText)
            ? _allClients
            : _allClients.Where(c =>
                c.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                (c.CompanyName?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Email?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var c in filtered)
            Clients.Add(c);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
