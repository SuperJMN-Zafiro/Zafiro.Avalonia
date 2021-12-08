using System.Numerics;
using Avalonia;

namespace Zafiro.Avalonia;

public static class RectMixin
{
    public static Rect Multiply(this Rect rect, Vector2 multiplier)
    {
        var position = new Point(rect.X * multiplier.X, rect.Y * multiplier.Y);
        var size = new Size(rect.Width * multiplier.X, rect.Height * multiplier.Y);
        return new Rect(position, size);
    }
}