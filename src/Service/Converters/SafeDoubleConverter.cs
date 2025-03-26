using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace src.Service.Converters;

public class SafeDoubleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double v)
        {
            return v.ToString("F0");
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? text = value?.ToString();
        
        if (string.IsNullOrEmpty(text))
        {
            return double.NaN;
        }

        if (!double.TryParse(text, NumberStyles.Any, culture, out var _))
        {
            return double.NaN;
        }
        
        var result = double.Parse(text);
        return result;
    }
}