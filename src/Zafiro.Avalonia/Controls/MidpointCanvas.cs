namespace Zafiro.Avalonia.Controls;

public class MidpointCanvas : Canvas
{
    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            var desiredSize = child.DesiredSize;

            var left = GetLeft(child);
            var top = GetTop(child);

            left = double.IsNaN(left) ? 0 : left;
            top = double.IsNaN(top) ? 0 : top;

            var rect = new Rect(left - desiredSize.Width / 2, top - desiredSize.Height / 2, desiredSize.Width, desiredSize.Height);

            child.Arrange(rect);
        }

        return finalSize;
    }
}