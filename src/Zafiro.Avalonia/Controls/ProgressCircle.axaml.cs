using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

public class ProgressCircle : Control
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<ProgressCircle, double>(nameof(Progress));

    public static readonly StyledProperty<IBrush> FillProperty =
        AvaloniaProperty.Register<ProgressCircle, IBrush>(nameof(Fill));

    public static readonly StyledProperty<IBrush> StrokeProperty =
        AvaloniaProperty.Register<ProgressCircle, IBrush>(nameof(Stroke));
    
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ProgressCircle, double>(nameof(StrokeThickness));

    private Size renderSize = new Size(0,0);

    static ProgressCircle()
    {
        AffectsRender<ProgressCircle>(ProgressProperty);
        AffectsRender<ProgressCircle>(FillProperty);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public IBrush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public IBrush Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }
    
    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        renderSize = finalSize;
        return base.ArrangeOverride(finalSize);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = new Rect(0,0, renderSize.Width, renderSize.Height);
        var center = bounds.Center;
        var radius = Math.Min(bounds.Width, bounds.Height) / 2;
        
        // Progress slice
        if (Progress > 0)
        {
            if (Progress >= 1)
            {
                // Draw a full circle
                context.DrawEllipse(Fill, null, center, radius, radius);
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

                context.DrawGeometry(Fill, null, geometry);
            }
        }
        
        // Background circle
        context.DrawEllipse(null, new Pen(Stroke, StrokeThickness), center, radius - StrokeThickness /2, radius - StrokeThickness/2);
    }
}