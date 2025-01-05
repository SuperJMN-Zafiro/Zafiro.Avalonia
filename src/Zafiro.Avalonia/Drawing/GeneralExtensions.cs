using Avalonia.Layout;
using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing;

public static class GeneralExtensions
{
    public static void Connect(this DrawingContext context, Visual parent, Visual from, Visual to,
        VerticalAlignment fromVerticalAlignment, HorizontalAlignment fromHorizontalAlignment,
        VerticalAlignment toVerticalAlignment, HorizontalAlignment toHorizontalAlignment, ILineStrategy strategy, Pen pen)
    {
        var fromPoint = from.GetAlignedPoint(fromVerticalAlignment, fromHorizontalAlignment, parent);
        var toPoint = to.GetAlignedPoint(toVerticalAlignment, toHorizontalAlignment, parent);

        strategy.Draw(context, fromPoint, toPoint, pen);
    }

    public static Point GetAlignedPoint(this Visual visual, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Visual reference)
    {
        var bounds = visual.Bounds;
        double x = CalculateHorizontalOffset(bounds.Width, horizontalAlignment);
        double y = CalculateVerticalOffset(bounds.Height, verticalAlignment);

        var point = new Point(x, y);
        return visual.TranslatePoint(point, reference) ?? new Point();
    }

    private static double CalculateHorizontalOffset(double width, HorizontalAlignment alignment)
    {
        return alignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Center => width / 2,
            HorizontalAlignment.Right => width,
            _ => throw new ArgumentException("Invalid horizontal alignment"),
        };
    }

    private static double CalculateVerticalOffset(double height, VerticalAlignment alignment)
    {
        return alignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Center => height / 2,
            VerticalAlignment.Bottom => height,
            _ => throw new ArgumentException("Invalid vertical alignment"),
        };
    }
    
    public static void DrawSmoothCurve(this DrawingContext context, IList<Point> points, Pen pen)
    {
        if (points.Count < 2)
            return;

        var geometry = new StreamGeometry();

        using (var geometryContext = geometry.Open())
        {
            geometryContext.BeginFigure(points[0], false);

            for (int i = 0; i < points.Count - 1; i++)
            {
                // Calcular puntos de control
                var cp1 = new Point((points[i].X + points[i + 1].X) / 2, points[i].Y);
                var cp2 = new Point((points[i].X + points[i + 1].X) / 2, points[i + 1].Y);
                var endPoint = points[i + 1];

                geometryContext.CubicBezierTo(cp1, cp2, endPoint);
            }
        }

        context.DrawGeometry(null, pen, geometry);
    }
}