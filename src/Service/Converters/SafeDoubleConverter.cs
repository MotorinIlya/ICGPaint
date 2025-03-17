using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace src.Service.Converters;

public class SafeDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return double.TryParse(value?.ToString(), NumberStyles.Any, culture, out double result) 
            ? result 
            : 0;
    }
}