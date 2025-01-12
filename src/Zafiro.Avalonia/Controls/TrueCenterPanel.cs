using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Zafiro.Avalonia.Controls;

public enum TrueCenterDock
{
    Left,
    Center,
    Right
}

public class TrueCenterPanel : Panel
{
    public static readonly AttachedProperty<TrueCenterDock> DockProperty =
        AvaloniaProperty.RegisterAttached<TrueCenterPanel, Control, TrueCenterDock>(
            "Dock",
            defaultValue: TrueCenterDock.Left
        );

    public static TrueCenterDock GetDock(Control element)
        => element.GetValue(DockProperty);

    public static void SetDock(Control element, TrueCenterDock value)
        => element.SetValue(DockProperty, value);

    protected override Size MeasureOverride(Size availableSize)
    {
        var leftChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Left);
        var centerChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Center);
        var rightChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Right);

        // 1) Mide Left y Right libremente con el tamaño ofrecido
        leftChild?.Measure(availableSize);
        var leftSize = leftChild?.DesiredSize ?? new Size();

        rightChild?.Measure(availableSize);
        var rightSize = rightChild?.DesiredSize ?? new Size();

        // 2) side = máximo ancho de left vs right
        double side = Math.Max((double)leftSize.Width, rightSize.Width);

        // 3) El ancho disponible para el centro = total - 2*side (clamp >=0)
        double centerAvailWidth = Math.Max(0, availableSize.Width - 2 * side);

        // Mide el centro con ese ancho
        if (centerChild != null)
        {
            centerChild.Measure(new Size(centerAvailWidth, availableSize.Height));
        }

        var centerSize = centerChild?.DesiredSize ?? new Size();

        // 4) La altura necesaria = la máxima de los tres
        double neededHeight = new double[] { leftSize.Height, centerSize.Height, rightSize.Height }.Max();
        double finalHeight = Math.Min(neededHeight, availableSize.Height);

        // Pedimos todo el ancho que haya
        return new Size(availableSize.Width, finalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var leftChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Left);
        var centerChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Center);
        var rightChild = Children.FirstOrDefault(c => GetDock(c) == TrueCenterDock.Right);

        var leftSize = leftChild?.DesiredSize ?? new Size();
        var centerSize = centerChild?.DesiredSize ?? new Size();
        var rightSize = rightChild?.DesiredSize ?? new Size();

        // side = mayor ancho de Left / Right
        double side = Math.Max(leftSize.Width, rightSize.Width);

        // --- LEFT ---
        if (leftChild != null)
        {
            double w = Math.Min(leftSize.Width, finalSize.Width);
            // Calculamos la altura final que va a usar
            double h = GetArrangedHeight(leftChild, finalSize.Height, leftSize.Height);
            // Calculamos el top en función del VerticalAlignment
            double top = GetTop(leftChild, finalSize.Height, h);

            leftChild.Arrange(new Rect(0, top, w, h));
        }

        // --- RIGHT ---
        if (rightChild != null)
        {
            double w = Math.Min(rightSize.Width, finalSize.Width);
            double x = Math.Max(0, finalSize.Width - w);

            double h = GetArrangedHeight(rightChild, finalSize.Height, rightSize.Height);
            double top = GetTop(rightChild, finalSize.Height, h);

            rightChild.Arrange(new Rect(x, top, w, h));
        }

        // --- CENTER ---
        if (centerChild != null)
        {
            // Franja central = [side .. (finalSize.Width - side)]
            double sliceWidth = Math.Max(0, finalSize.Width - 2 * side);
            double cWidth = Math.Min(centerSize.Width, sliceWidth);

            double cHeight = GetArrangedHeight(centerChild, finalSize.Height, centerSize.Height);
            double top = GetTop(centerChild, finalSize.Height, cHeight);

            // centrar dentro de esa franja
            double cX = side + (sliceWidth - cWidth) / 2;
            centerChild.Arrange(new Rect(cX, top, cWidth, cHeight));
        }

        return finalSize;
    }

    /// <summary>
    /// Calcula la posición vertical (Top) según el VerticalAlignment del control.
    /// </summary>
    private double GetTop(Control child, double containerHeight, double arrangedHeight)
    {
        return child.VerticalAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => containerHeight - arrangedHeight,
            VerticalAlignment.Center => (containerHeight - arrangedHeight) / 2,
            VerticalAlignment.Stretch => 0, // y el alto se lleva todo
            _ => 0
        };
    }

    /// <summary>
    /// Calcula el alto final que debe ocupar el child, según su VerticalAlignment.
    /// </summary>
    private double GetArrangedHeight(Control child, double containerHeight, double measuredHeight)
    {
        return child.VerticalAlignment == VerticalAlignment.Stretch
            ? containerHeight
            : measuredHeight;
    }
}