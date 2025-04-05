using System.Globalization;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class EnumMappingConverter : IValueConverter
{
    public Dictionary<object, object> EnumMappings { get; set; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || !value.GetType().IsEnum)
        {
            return null;
        }

        if (EnumMappings.TryGetValue(value, out var mappedValue))
        {
            return mappedValue;
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}