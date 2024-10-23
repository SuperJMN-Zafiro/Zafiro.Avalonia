using Avalonia.Layout;
using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams.Drawing;

public static class DrawingContextExtensions
{
    public static void Connect(this DrawingContext context, Visual parent, Visual from, Visual to,
        VerticalAlignment fromVerticalAlignment, HorizontalAlignment fromHorizontalAlignment,
        VerticalAlignment toVerticalAlignment, HorizontalAlignment toHorizontalAlignment, ILineStrategy strategy, Pen pen)
    {
        var fromPoint = GetAlignedPoint(from, fromVerticalAlignment, fromHorizontalAlignment, parent, "from");
        var toPoint = GetAlignedPoint(to, toVerticalAlignment, toHorizontalAlignment, parent, "to");

        strategy.Draw(context, fromPoint, toPoint, pen);
    }

    private static Point GetAlignedPoint(Visual visual, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Visual reference, string visualName)
    {
        var bounds = visual.Bounds;
        double x = CalculateHorizontalOffset(bounds.Width, horizontalAlignment, visualName);
        double y = CalculateVerticalOffset(bounds.Height, verticalAlignment, visualName);

        var point = new Point(x, y);
        return visual.TranslatePoint(point, reference) ?? new Point();
    }

    private static double CalculateHorizontalOffset(double width, HorizontalAlignment alignment, string visualName)
    {
        return alignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Center => width / 2,
            HorizontalAlignment.Right => width,
            _ => throw new ArgumentException($"Invalid horizontal alignment for '{visualName}'"),
        };
    }

    private static double CalculateVerticalOffset(double height, VerticalAlignment alignment, string visualName)
    {
        return alignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Center => height / 2,
            VerticalAlignment.Bottom => height,
            _ => throw new ArgumentException($"Invalid vertical alignment for '{visualName}'"),
        };
    }
}