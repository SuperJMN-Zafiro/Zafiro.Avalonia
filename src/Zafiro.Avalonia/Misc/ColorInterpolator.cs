using Avalonia.Media;

namespace Zafiro.Avalonia.ViewLocators;

public static class ColorInterpolator
{
    public static Color InterpolateColor(List<Color> colors, double x)
    {
        if (colors == null || colors.Count == 0)
            throw new ArgumentException("The color list cannot be null or empty.");

        // Limita x a un rango de 0 a 1
        x = Math.Max(0, Math.Min(1, x));

        // Calcula la posición exacta en la lista de colores
        double scaledPosition = x * (colors.Count - 1);
        int index = (int)scaledPosition;
        double localX = scaledPosition - index;

        // Si está exactamente en un color, devuélvelo directamente
        if (index >= colors.Count - 1)
            return colors[^1];

        // Obtén los dos colores entre los cuales interpolaremos
        Color colorA = colors[index];
        Color colorB = colors[index + 1];

        // Interpola entre los valores RGBA de colorA y colorB
        byte a = (byte)(colorA.A + (colorB.A - colorA.A) * localX);
        byte r = (byte)(colorA.R + (colorB.R - colorA.R) * localX);
        byte g = (byte)(colorA.G + (colorB.G - colorA.G) * localX);
        byte b = (byte)(colorA.B + (colorB.B - colorA.B) * localX);

        return Color.FromArgb(a, r, g, b);
    }
}