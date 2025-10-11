using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Converters;

public sealed class PreferredHeightConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var fallbackHeight = TryGetDouble(parameter).GetValueOrDefault(0d);

        var firstPositive = values
            .Select(TryGetDouble)
            .Select(maybe => maybe.Where(height => height > 0))
            .FirstOrDefault(maybe => maybe.HasValue);

        return firstPositive.HasValue ? firstPositive.Value : fallbackHeight;
    }

    public object? ConvertBack(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }

    private static Maybe<double> TryGetDouble(object? value)
    {
        return value switch
        {
            double number => number,
            float number => (double)number,
            string text when double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) => parsed,
            UnsetValueType => Maybe<double>.None,
            null => Maybe<double>.None,
            _ => Maybe<double>.None,
        };
    }
}
