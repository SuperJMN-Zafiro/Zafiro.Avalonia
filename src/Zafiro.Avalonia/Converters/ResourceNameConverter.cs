using System.Globalization;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class ResourceNameConverter : IValueConverter
{
    public static ResourceNameConverter Instance { get; } = new();

    private ResourceNameConverter()
    {
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value != null && Application.Current != null && Application.Current.Resources.TryGetResource(value, null, out var resource))
        {
            return resource;
        }
        
        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}