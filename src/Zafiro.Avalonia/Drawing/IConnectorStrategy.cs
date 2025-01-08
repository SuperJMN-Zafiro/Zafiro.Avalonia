using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing;

public interface IConnectorStrategy
{
    public void Draw(DrawingContext context, Pen pen, Point from, Side fromSide, Point to, Side toSide,
        bool startArrow = false, bool endArrow = false);
    
    Point LabelPosition(Point from, Side fromSide, Point to, Side toSide);
}