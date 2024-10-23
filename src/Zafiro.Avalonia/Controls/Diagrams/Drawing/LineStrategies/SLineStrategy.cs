using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams.Drawing.LineStrategies;

public class SLineStrategy : ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen)
    {
        context.SLine(from, to, pen);
    }

    public static ILineStrategy Instance { get; } = new SLineStrategy();
}