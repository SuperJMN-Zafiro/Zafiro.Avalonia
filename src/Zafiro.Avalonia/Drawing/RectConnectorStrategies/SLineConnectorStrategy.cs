using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing.RectConnectorStrategies;

public class SLineConnectorStrategy : IConnectorStrategy
{
    public static SLineConnectorStrategy Instance { get; } = new();

    public void Draw(DrawingContext context, Pen pen, Point from, Side fromSide, Point to, Side toSide, bool startArrow = false, bool endArrow = false)
    {
        context.ConnectWithSLine(from, fromSide, to, toSide, pen, startArrow, endArrow);
    }
    
    public Point LabelPosition(Point from, Side fromSide, Point to, Side toSide)
    {
        var midPoint = new Point((from.X + to.X) / 2, (from.Y + to.Y) / 2);
        return midPoint;
    }
}