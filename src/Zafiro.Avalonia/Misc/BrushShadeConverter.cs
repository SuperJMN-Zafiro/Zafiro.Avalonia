using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Zafiro.Avalonia.Misc;

public sealed class BrushShadeConverter : IValueConverter
{
    public double DeltaHue { get; set; } = 0.0; // degrees
    public double DeltaLightness { get; set; } = 0.0; // -1..+1
    public double SaturationScale { get; set; } = 1.0;
    public double Alpha { get; set; } = 1.0; // 0..1 multiplier over effective alpha

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IBrush b) return AvaloniaProperty.UnsetValue;
        if (!BrushColor.TryGet(b, out var c, out var effA)) return AvaloniaProperty.UnsetValue;

        // Color -> HSL
        var hsl = c.ToHsl();
        var h = (hsl.H + DeltaHue) % 360.0;
        if (h < 0) h += 360.0;
        var s = Math.Clamp(hsl.S * SaturationScale, 0, 1);
        var l = Math.Clamp(hsl.L + DeltaLightness, 0, 1);

        // HSL -> Color (RGB with original alpha first)
        var rgb = HslColor.FromHsl(h, s, l).ToRgb();
        // Apply target alpha over effective input alpha
        var outA = Math.Clamp(effA * Alpha, 0, 1);
        var outColor = BrushColor.WithAlpha(rgb, outA);

        return new ImmutableSolidColorBrush(outColor);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}