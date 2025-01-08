namespace Zafiro.Avalonia.Controls.Diagrams;

public static class RectExtensions
{
    public static Point MiddlePoint(this Rect rect)
    {
        return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }
}