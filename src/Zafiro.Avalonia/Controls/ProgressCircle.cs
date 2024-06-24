using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

using System;

public class ProgressCircle : Control
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<ProgressCircle, double>(nameof(Progress), 0.0);

    static ProgressCircle()
    {
        AffectsRender<ProgressCircle>(ProgressProperty);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = this.Bounds;
        var center = bounds.Center;
        var radius = Math.Min(bounds.Width, bounds.Height) / 2 - 2; // Ajustar el radio para evitar el clipping

        // Background circle
        context.DrawEllipse(null, new Pen(Brushes.Gray, 2), center, radius, radius);

        // Progress slice
        if (Progress > 0)
        {
            if (Progress >= 1)
            {
                // Draw a full circle
                context.DrawEllipse(Brushes.Green, null, center, radius, radius);
            }
            else
            {
                var angle = 360 * Progress;
                var startAngle = -90; // Start from the top
                var endAngle = startAngle + angle;

                var geometry = new StreamGeometry();
                using (var contextGeometry = geometry.Open())
                {
                    contextGeometry.BeginFigure(center, true);

                    // Draw the line to the edge of the circle
                    var startPoint = center + new Vector(Math.Cos(Math.PI * startAngle / 180) * radius,
                        Math.Sin(Math.PI * startAngle / 180) * radius);
                    contextGeometry.LineTo(startPoint);

                    // Draw the arc
                    var largeArc = angle > 180.0;
                    contextGeometry.ArcTo(
                        center + new Vector(Math.Cos(Math.PI * endAngle / 180) * radius,
                            Math.Sin(Math.PI * endAngle / 180) * radius),
                        new Size(radius, radius),
                        0,
                        largeArc,
                        SweepDirection.Clockwise);

                    // Close the slice
                    contextGeometry.EndFigure(true);
                }

                context.DrawGeometry(Brushes.Green, null, geometry);
            }
        }
    }
}