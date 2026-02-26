using System;
using System.Globalization;
using Avalonia.Data.Converters;
using HealthCenter.Desktop.Database.Entities;

namespace HealthCenter.Desktop.Infrastructure.Converters;

public class TicketStatusConverter : IValueConverter
{
    public static readonly TicketStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Waiting => "في الانتظار",
                TicketStatus.Called => "تم النداء",
                TicketStatus.InProgress => "قيد الفحص",
                TicketStatus.AwaitingRecall => "بانتظار إعادة النداء",
                TicketStatus.Completed => "منتهي",
                TicketStatus.Present => "حاضر",
                _ => value.ToString()
            };
        }
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
