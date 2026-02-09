using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HealthCenter.Desktop.Features.Queue.Converters;

public class MuteIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMuted)
        {
            return isMuted ? "🔇" : "🔊";
        }
        return "🔊";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
