using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Dialogs;

// Computes a uniform Thickness padding based on the current available size.
// Parameters (optional, as a string): "factor,min,max"
// - factor: proportion of the shortest side used for padding (default: 0.04 => 4%)
// - min: lower clamp (default: 6)
// - max: upper clamp (default: 20)
public sealed class ResponsivePaddingConverter : IMultiValueConverter
{
    public static ResponsivePaddingConverter Instance { get; } = new();

    public object Convert(IList<object> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // Expecting: [width, height]
        if (values.Count < 2 || values[0] is UnsetValueType || values[1] is UnsetValueType)
        {
            return new Thickness(16); // Fallback to legacy default
        }

        if (values[0] is not double width || values[1] is not double height)
        {
            return new Thickness(16);
        }

        // Defaults
        var factor = 0.04; // 4% of the shortest side
        var min = 6.0;
        var max = 20.0;

        // Optional parameter parsing: "factor,min,max"
        if (parameter is string s && !string.IsNullOrWhiteSpace(s))
        {
            var parts = s.Split(',');
            if (parts.Length > 0 && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
                factor = f;
            if (parts.Length > 1 && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var mi))
                min = mi;
            if (parts.Length > 2 && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var ma))
                max = ma;
        }

        var shortest = Math.Max(0, Math.Min(width, height));
        var padding = shortest * factor;
        padding = Math.Clamp(padding, min, max);

        return new Thickness(padding);
    }
}