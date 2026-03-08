using System;
using System.Globalization;
using Avalonia.Data.Converters;
using HealthCenter.Desktop.Database.Entities;

namespace HealthCenter.Desktop.Converters;

public class TicketStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TicketStatus status)
            return string.Empty;

        return status switch
        {
            TicketStatus.Waiting => "في الانتظار",
            TicketStatus.Called => "تم النداء",
            TicketStatus.Present => "حضر",
            TicketStatus.Absent => "غائب",
            TicketStatus.AwaitingRecall => "في قائمة الغائبين",
            TicketStatus.InProgress => "قيد المعاينة",
            TicketStatus.Completed => "تمت الزيارة",
            _ => status.ToString()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

