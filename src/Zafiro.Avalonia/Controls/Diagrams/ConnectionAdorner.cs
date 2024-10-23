using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class ConnectionAdorner : Control
{
    public static readonly StyledProperty<Control?> FromProperty =
        AvaloniaProperty.Register<ConnectionAdorner, Control?>(nameof(From));

    public static readonly StyledProperty<Control?> ToProperty =
        AvaloniaProperty.Register<ConnectionAdorner, Control?>(nameof(To));

    public Visual? From
    {
        get => GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    public Visual? To
    {
        get => GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (From == null || To == null)
        {
            return;
        }

        // Obtener los rectángulos de los controles "From" y "To"
        var fromBounds = From.Bounds;
        var toBounds = To.Bounds;

        // Obtener las posiciones absolutas en el contenedor del adorner
        var fromPosition = From.TranslatePoint(new Point(fromBounds.Width / 2, fromBounds.Height / 2), this) ?? new Point();
        var toPosition = To.TranslatePoint(new Point(toBounds.Width / 2, toBounds.Height / 2), this) ?? new Point();

        // Dibujar la línea entre los puntos
        context.DrawLine(new Pen(Brushes.Black, 2), fromPosition, toPosition);
    }
}
