using Avalonia.Media;

namespace Zafiro.Avalonia.Drawing;

public static class LineExtensions
{
    public static void OrthogonalLine(this DrawingContext context, Point pointA, Point pointB, Pen pen)
    {
        var intermediatePoint = new Point(pointA.X, pointB.Y);

        var segments = new PathSegments
        {
            new LineSegment { Point = intermediatePoint },
            new LineSegment { Point = pointB }
        };

        var figure = new PathFigure
        {
            StartPoint = pointA,
            Segments = segments,
            IsClosed = false,
            IsFilled = false
        };

        var geometry = new PathGeometry
        {
            Figures = new PathFigures { figure },
        };

        context.DrawGeometry(null, pen, geometry);
    }

    public static void CubicBezierLine(this DrawingContext context, Point pointA, Point pointB, Pen pen)
    {
        double offset = 50;
        Point controlPoint1 = new Point(pointA.X, pointA.Y - offset);
        Point controlPoint2 = new Point(pointB.X, pointB.Y + offset);

        var segment = new BezierSegment
        {
            Point1 = controlPoint1,
            Point2 = controlPoint2,
            Point3 = pointB,
        };

        var figure = new PathFigure
        {
            StartPoint = pointA,
            Segments = new PathSegments { segment },
            IsClosed = false,
        };

        var geometry = new PathGeometry
        {
            Figures = new PathFigures { figure }
        };

        context.DrawGeometry(null, pen, geometry);
    }

    public static void QuadraticBezierLine(this DrawingContext context, Point pointA, Point pointB, Pen pen)
    {
        double offset = 50;
        Point controlPoint = new Point(
            (pointA.X + pointB.X) / 2,
            (pointA.Y + pointB.Y) / 2 - offset
        );

        var segment = new QuadraticBezierSegment
        {
            Point1 = controlPoint,
            Point2 = pointB
        };

        var figure = new PathFigure
        {
            StartPoint = pointA,
            Segments = new PathSegments { segment },
            IsClosed = false,
        };

        var geometry = new PathGeometry
        {
            Figures = new PathFigures { figure }
        };

        context.DrawGeometry(null, pen, geometry);
    }

    public static void SLine(this DrawingContext context, Point pointA, Point pointB, Pen pen)
    {
        double offset = 100;
        Point controlPoint1 = new Point(pointA.X + offset, pointA.Y);
        Point controlPoint2 = new Point(pointB.X - offset, pointB.Y);

        var segment = new BezierSegment
        {
            Point1 = controlPoint1,
            Point2 = controlPoint2,
            Point3 = pointB
        };

        var figure = new PathFigure
        {
            StartPoint = pointA,
            Segments = new PathSegments { segment },
            IsClosed = false,
        };

        var geometry = new PathGeometry
        {
            Figures = new PathFigures { figure }
        };

        context.DrawGeometry(null, pen, geometry);
    }
}