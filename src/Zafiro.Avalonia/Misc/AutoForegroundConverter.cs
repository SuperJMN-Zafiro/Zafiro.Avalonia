using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Zafiro.Avalonia.Misc;

public sealed class AutoForegroundConverter : IValueConverter
{
    public double Bias { get; set; } = 0.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IBrush b) return AvaloniaProperty.UnsetValue;
        if (!BrushColor.TryGet(b, out var c, out _)) return AvaloniaProperty.UnsetValue;

        // sRGB relative luminance
        static double Lin(byte u)
        {
            var s = u / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        var L = 0.2126 * Lin(c.R) + 0.7152 * Lin(c.G) + 0.0722 * Lin(c.B);
        var threshold = 0.5 + Bias;
        var fg = L > threshold ? Colors.Black : Colors.White;

        return new ImmutableSolidColorBrush(fg);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}