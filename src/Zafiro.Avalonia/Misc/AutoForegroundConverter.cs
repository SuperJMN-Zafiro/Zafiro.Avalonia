using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Zafiro.Avalonia.Misc;

public sealed class AutoForegroundConverter : AvaloniaObject, IValueConverter
{
    public static readonly StyledProperty<IBrush> FallbackBackgroundProperty = AvaloniaProperty.Register<AutoForegroundConverter, IBrush>(
        nameof(FallbackBackground));

    public IBrush FallbackBackground
    {
        get => GetValue(FallbackBackgroundProperty);
        set => SetValue(FallbackBackgroundProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Pick background: use input if solid & alpha>~0, else fallback
        IBrush pick = value as IBrush;
        if (!(pick is ISolidColorBrush solid) || solid.Opacity <= 0.001 || solid.Color.A == 0)
            pick = FallbackBackground;

        if (!(pick is ISolidColorBrush fb)) return AvaloniaProperty.UnsetValue;
        var c = fb.Color;

        static double Lin(byte u)
        {
            var s = u / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        var L = 0.2126 * Lin(c.R) + 0.7152 * Lin(c.G) + 0.0722 * Lin(c.B);
        var fg = L > 0.5 ? Colors.Black : Colors.White;
        return new ImmutableSolidColorBrush(fg);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}