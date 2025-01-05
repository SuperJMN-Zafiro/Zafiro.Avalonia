using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing.LineStrategies;

public class OrthogonalStrategy : ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen)
    {
        context.OrthogonalLine(from, to, pen);
    }

    public static ILineStrategy Instance { get; } = new OrthogonalStrategy();
}