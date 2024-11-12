namespace Zafiro.Avalonia.Controls;

/// <summary>
/// A panel that arranges child elements around a central element, similar to a 3x3 grid.
/// Elements can be positioned to the left, right, top, bottom, or center, where the central
/// element fills the remaining available space. Anchored elements (left, right, top, bottom)
/// do not exceed the size of the central element along their aligned axis.
/// </summary>
/// <remarks>
/// Use the <see cref="PositionProperty"/> attached property to specify the position
/// of each child. Possible positions include Left, Right, Top, Bottom, and Center.
/// The panel adjusts the dimensions of anchored elements to ensure they stay within
/// the bounds of the central element.
/// </remarks>
public class CentralizedDockPanel : Panel
{
    public static readonly AttachedProperty<Position> PositionProperty =
        AvaloniaProperty.RegisterAttached<CentralizedDockPanel, Control, Position>("Position", Position.Center);

    public static Position GetPosition(Control element)
    {
        return element.GetValue(PositionProperty);
    }

    public static void SetPosition(Control element, Position position)
    {
        element.SetValue(PositionProperty, position);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double leftWidth = 0, rightWidth = 0, topHeight = 0, bottomHeight = 0;
        double centerWidth = 0, centerHeight = 0;

        // Medir los elementos anclados primero para obtener sus tamaños deseados
        foreach (var child in Children)
        {
            var position = GetPosition(child);
            Size constraint = availableSize;

            switch (position)
            {
                case Position.Left:
                case Position.Right:
                    // Limitar la altura al disponible menos espacios reservados
                    constraint = new Size(double.PositiveInfinity, availableSize.Height - topHeight - bottomHeight);
                    child.Measure(constraint);
                    break;
                case Position.Top:
                case Position.Bottom:
                    // Limitar el ancho al disponible menos espacios reservados
                    constraint = new Size(availableSize.Width - leftWidth - rightWidth, double.PositiveInfinity);
                    child.Measure(constraint);
                    break;
            }

            var desiredSize = child.DesiredSize;

            switch (position)
            {
                case Position.Left:
                    leftWidth = Math.Max(leftWidth, desiredSize.Width);
                    break;
                case Position.Right:
                    rightWidth = Math.Max(rightWidth, desiredSize.Width);
                    break;
                case Position.Top:
                    topHeight = Math.Max(topHeight, desiredSize.Height);
                    break;
                case Position.Bottom:
                    bottomHeight = Math.Max(bottomHeight, desiredSize.Height);
                    break;
            }
        }

        // Ahora medir el elemento central con el espacio restante
        Size centerAvailableSize = new Size(
            Math.Max(0, availableSize.Width - leftWidth - rightWidth),
            Math.Max(0, availableSize.Height - topHeight - bottomHeight));

        foreach (var child in Children)
        {
            if (GetPosition(child) == Position.Center)
            {
                child.Measure(centerAvailableSize);
                centerWidth = Math.Max(centerWidth, child.DesiredSize.Width);
                centerHeight = Math.Max(centerHeight, child.DesiredSize.Height);
            }
        }

        // Tamaño total necesario
        double totalWidth = leftWidth + Math.Max(centerWidth, centerAvailableSize.Width) + rightWidth;
        double totalHeight = topHeight + Math.Max(centerHeight, centerAvailableSize.Height) + bottomHeight;

        return new Size(
            Math.Min(totalWidth, availableSize.Width),
            Math.Min(totalHeight, availableSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double leftWidth = 0, rightWidth = 0, topHeight = 0, bottomHeight = 0;
        double centerWidth = Math.Max(0, finalSize.Width), centerHeight = Math.Max(0, finalSize.Height);

        // Primero, determinar los tamaños de los elementos anclados
        foreach (var child in Children)
        {
            var position = GetPosition(child);
            var desiredSize = child.DesiredSize;

            switch (position)
            {
                case Position.Left:
                    leftWidth = Math.Max(leftWidth, desiredSize.Width);
                    break;
                case Position.Right:
                    rightWidth = Math.Max(rightWidth, desiredSize.Width);
                    break;
                case Position.Top:
                    topHeight = Math.Max(topHeight, desiredSize.Height);
                    break;
                case Position.Bottom:
                    bottomHeight = Math.Max(bottomHeight, desiredSize.Height);
                    break;
            }
        }

        // Calcular el tamaño del elemento central
        centerWidth = Math.Max(0, finalSize.Width - leftWidth - rightWidth);
        centerHeight = Math.Max(0, finalSize.Height - topHeight - bottomHeight);

        // Disponer los elementos
        foreach (var child in Children)
        {
            var position = GetPosition(child);
            var desiredSize = child.DesiredSize;
            Rect rect = new Rect();

            switch (position)
            {
                case Position.Left:
                    rect = new Rect(0, topHeight, desiredSize.Width, centerHeight);
                    break;
                case Position.Right:
                    rect = new Rect(finalSize.Width - desiredSize.Width, topHeight, desiredSize.Width, centerHeight);
                    break;
                case Position.Top:
                    rect = new Rect(leftWidth, 0, centerWidth, desiredSize.Height);
                    break;
                case Position.Bottom:
                    rect = new Rect(leftWidth, finalSize.Height - desiredSize.Height, centerWidth, desiredSize.Height);
                    break;
                case Position.Center:
                    rect = new Rect(leftWidth, topHeight, centerWidth, centerHeight);
                    break;
            }

            child.Arrange(rect);
        }

        return finalSize;
    }
}