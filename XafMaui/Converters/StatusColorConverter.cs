using System.Globalization;
using XafMaui.Models;

namespace XafMaui.Converters;

public class StatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value is int i ? (TimeEntryStatus)i : TimeEntryStatus.Draft;
        return status switch
        {
            TimeEntryStatus.Draft => Colors.Gray,
            TimeEntryStatus.Submitted => Color.FromArgb("#FFA726"),  // amber
            TimeEntryStatus.Approved => Color.FromArgb("#66BB6A"),   // green
            TimeEntryStatus.Rejected => Color.FromArgb("#EF5350"),   // red
            _ => Colors.Gray,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
