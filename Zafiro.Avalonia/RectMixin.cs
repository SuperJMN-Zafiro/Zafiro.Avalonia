using System.Numerics;
using Avalonia;

namespace Zafiro.Avalonia;

public static class RectMixin
{
    public static Rect Multiply(this Rect rect, Vector2 v)
    {
        var position = new Point(Coerce(rect.X * v.X), Coerce(rect.Y * v.Y));
        var size = new Size(Coerce(rect.Width * v.X), Coerce(rect.Height * v.Y));
        return new Rect(position, size);
    }

    private static double Coerce(double value)
    {
        return double.IsNaN(value) ? 0d : value;
    }
}