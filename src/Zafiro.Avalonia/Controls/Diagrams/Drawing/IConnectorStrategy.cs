using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams.Drawing;

public interface IConnectorStrategy
{
    public void Draw(DrawingContext context, Pen pen, Point from, Side fromSide, Point to, Side toSide,
        bool startArrow = false, bool endArrow = false);
}