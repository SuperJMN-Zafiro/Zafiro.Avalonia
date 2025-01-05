using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing;

public interface ILineStrategy
{
    public void Draw(DrawingContext context, Point from, Point to, Pen pen);
}