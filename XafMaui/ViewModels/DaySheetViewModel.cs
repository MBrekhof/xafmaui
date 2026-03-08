using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.ViewModels;

public class DaySheetViewModel : INotifyPropertyChanged
{
    DateTime _selectedDate = DateTime.Today;

    public ObservableCollection<LocalTimeEntry> TimeEntries { get; } = [];

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set { _selectedDate = value; OnPropertyChanged(); LoadEntries(); }
    }

    public decimal TotalHours => TimeEntries.Sum(t => t.Hours);

    public void LoadEntries()
    {
        TimeEntries.Clear();
        using var db = new LocalDbContext();
        var entries = db.TimeEntries
            .Where(t => t.Date.Date == _selectedDate.Date)
            .OrderBy(t => t.ProjectName)
            .ThenBy(t => t.ProjectTaskName)
            .ToList();

        foreach (var e in entries)
            TimeEntries.Add(e);

        OnPropertyChanged(nameof(TotalHours));
    }

    public void SubmitDraftEntries()
    {
        using var db = new LocalDbContext();
        var drafts = db.TimeEntries
            .Where(t => t.Date.Date == _selectedDate.Date && t.Status == (int)TimeEntryStatus.Draft)
            .ToList();

        foreach (var entry in drafts)
        {
            entry.Status = (int)TimeEntryStatus.Submitted;
            entry.IsPendingSync = true;
        }
        db.SaveChanges();
        LoadEntries();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
