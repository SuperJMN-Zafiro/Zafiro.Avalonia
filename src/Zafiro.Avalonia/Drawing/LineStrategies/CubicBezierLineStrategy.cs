using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing.LineStrategies;

public class CubicBezierLineStrategy : ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen)
    {
        context.CubicBezierLine(from, to, pen);
    }

    public static ILineStrategy Instance { get; } = new CubicBezierLineStrategy();
}