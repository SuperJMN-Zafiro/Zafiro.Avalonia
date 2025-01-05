using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing.LineStrategies;

public class StraightLineStrategy : ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen)
    {
        context.DrawLine(pen, from, to);
    }
}