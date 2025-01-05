using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing.RectConnectorStrategies;

public class SLineConnectorStrategy : IConnectorStrategy
{
    public void Draw(DrawingContext context, Pen pen, Point from, Side fromSide, Point to, Side toSide, bool startArrow = false, bool endArrow = false)
    {
        context.ConnectWithSLine(from, fromSide, to, toSide, pen, startArrow, endArrow);
    }

    public static SLineConnectorStrategy Instance { get; } = new();
}