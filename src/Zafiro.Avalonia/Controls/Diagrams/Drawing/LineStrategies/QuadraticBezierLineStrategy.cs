using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams.Drawing.LineStrategies;

public class QuadraticBezierLineStrategy : ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen)
    {
        context.QuadraticBezierLine(from, to, pen);
    }

    public static ILineStrategy Instance { get; } = new QuadraticBezierLineStrategy();
}