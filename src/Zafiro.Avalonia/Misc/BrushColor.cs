using Avalonia.Media;

namespace Zafiro.Avalonia.Misc;

static class BrushColor
{
    // Extracts solid color and effective alpha from an IBrush
    public static bool TryGet(IBrush brush, out Color color, out double effectiveAlpha)
    {
        if (brush is ISolidColorBrush scb)
        {
            color = scb.Color;
            // Effective alpha = color alpha * brush opacity
            effectiveAlpha = (color.A / 255.0) * scb.Opacity;
            return true;
        }

        color = default;
        effectiveAlpha = 1.0;
        return false;
    }

    // Builds a Color with the given alpha (0..1) preserving RGB
    public static Color WithAlpha(Color rgb, double a01)
    {
        var a = (byte)Math.Round(Math.Clamp(a01, 0, 1) * 255);
        return Color.FromArgb(a, rgb.R, rgb.G, rgb.B);
    }
}