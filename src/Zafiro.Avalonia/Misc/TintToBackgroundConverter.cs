using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Zafiro.Avalonia.Misc;

public sealed class TintToBackgroundConverter : AvaloniaObject, IValueConverter
{
    public static readonly StyledProperty<IBrush?> BaseBrushProperty =
        AvaloniaProperty.Register<TintToBackgroundConverter, IBrush?>(nameof(BaseBrush));

    public static readonly StyledProperty<double> StrengthProperty =
        AvaloniaProperty.Register<TintToBackgroundConverter, double>(nameof(Strength), 0.35);

    public IBrush? BaseBrush
    {
        get => GetValue(BaseBrushProperty);
        set => SetValue(BaseBrushProperty, value);
    }

    public double Strength
    {
        get => GetValue(StrengthProperty);
        set => SetValue(StrengthProperty, value);
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IBrush tint || BaseBrush is not IBrush baseBrush)
            return AvaloniaProperty.UnsetValue;

        if (!BrushColor.TryGet(tint, out var tintColor, out var tintEffA))
            return AvaloniaProperty.UnsetValue;
        if (!BrushColor.TryGet(baseBrush, out var baseColor, out _))
            return AvaloniaProperty.UnsetValue;

        var t = Math.Clamp(Strength * tintEffA, 0, 1);

        byte Lerp(byte a, byte b, double tt) => (byte)Math.Clamp(Math.Round(a + (b - a) * tt), 0, 255);

        var outColor = Color.FromArgb(255,
            Lerp(baseColor.R, tintColor.R, t),
            Lerp(baseColor.G, tintColor.G, t),
            Lerp(baseColor.B, tintColor.B, t));

        return new ImmutableSolidColorBrush(outColor);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}