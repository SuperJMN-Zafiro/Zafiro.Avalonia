using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams.Drawing;

public static class ConnectorExtensions
{
    public static void ConnectWithSLine(this DrawingContext context, Point from, Side sideFrom, Point to, Side sideTo,
        Pen pen, bool startArrow = false, bool endArrow = false)
    {
        // Definir el offset para el control de la curva
        double offset = 100;
        Point controlPoint1 = from;
        Point controlPoint2 = to;

        // Ajustar los puntos de control dependiendo del lado de conexión
        switch (sideFrom)
        {
            case Side.Top:
                controlPoint1 = new Point(from.X, from.Y - offset);
                break;
            case Side.Bottom:
                controlPoint1 = new Point(from.X, from.Y + offset);
                break;
            case Side.Left:
                controlPoint1 = new Point(from.X - offset, from.Y);
                break;
            case Side.Right:
                controlPoint1 = new Point(from.X + offset, from.Y);
                break;
        }

        switch (sideTo)
        {
            case Side.Top:
                controlPoint2 = new Point(to.X, to.Y - offset);
                break;
            case Side.Bottom:
                controlPoint2 = new Point(to.X, to.Y + offset);
                break;
            case Side.Left:
                controlPoint2 = new Point(to.X - offset, to.Y);
                break;
            case Side.Right:
                controlPoint2 = new Point(to.X + offset, to.Y);
                break;
        }

        // Crear la curva de Bézier
        var segment = new BezierSegment
        {
            Point1 = controlPoint1,
            Point2 = controlPoint2,
            Point3 = to
        };

        var figure = new PathFigure
        {
            StartPoint = from,
            Segments = new PathSegments {segment},
            IsClosed = false,
        };

        var geometry = new PathGeometry
        {
            Figures = new PathFigures {figure}
        };

        // Dibujar la curva
        context.DrawGeometry(null, pen, geometry);

        // Dibujar flechas si están habilitadas
        if (startArrow)
        {
            DrawArrowHead(context, controlPoint1, from, pen);
        }

        if (endArrow)
        {
            DrawArrowHead(context, controlPoint2, to, pen);
        }
    }


    private static void DrawArrowHead(DrawingContext context, Point controlPoint, Point endPoint, Pen pen)
    {
        // Cálculo de la dirección en el punto final
        Vector direction = endPoint - controlPoint;
        direction = direction.Normalize();

        // Vector perpendicular para la base de la flecha
        Vector perpendicular = new Vector(-direction.Y, direction.X);

        // Tamaño de la punta de la flecha
        double arrowSize = 10;

        // Puntos para el triángulo de la flecha
        Point arrowPoint1 = endPoint - direction * arrowSize + perpendicular * (arrowSize / 2);
        Point arrowPoint2 = endPoint - direction * arrowSize - perpendicular * (arrowSize / 2);

        // Crear la figura de la flecha
        var arrowFigure = new PathFigure
        {
            StartPoint = endPoint,
            Segments = new PathSegments
            {
                new LineSegment {Point = arrowPoint1},
                new LineSegment {Point = arrowPoint2}
            },
            IsClosed = true
        };

        var arrowGeometry = new PathGeometry
        {
            Figures = new PathFigures {arrowFigure}
        };

        // Dibujar la punta de la flecha
        context.DrawGeometry(pen.Brush, null, arrowGeometry);
    }

}