// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 
// 
// Licensed to The Avalonia Project under MIT License, courtesy of The .NET Foundation.

using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Utilities;
using static System.Math;

namespace Zafiro.Avalonia.Controls;

/// <summary>
///     Positions child elements in sequential position from left to right,
///     breaking content to the next line at the edge of the containing box.
///     Subsequent ordering happens sequentially from top to bottom or from right to left,
///     depending on the value of the <see cref="Orientation" /> property.
/// </summary>
public class WrapPanelEnhanced : Panel, INavigableContainer
{
    /// <summary>
    ///     Defines the <see cref="Orientation" /> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<WrapPanelEnhanced, Orientation>(nameof(Orientation));

    /// <summary>
    ///     Defines the <see cref="ItemWidth" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<WrapPanelEnhanced, double>(nameof(ItemWidth), double.NaN);

    /// <summary>
    ///     Defines the <see cref="ItemHeight" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<WrapPanelEnhanced, double>(nameof(ItemHeight), double.NaN);

    /// <summary>
    ///     Initializes static members of the <see cref="WrapPanelEnhanced" /> class.
    /// </summary>
    static WrapPanelEnhanced()
    {
        AffectsMeasure<WrapPanelEnhanced>(OrientationProperty, ItemWidthProperty, ItemHeightProperty);
    }

    /// <summary>
    ///     Gets or sets the orientation in which child controls will be layed out.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    ///     Gets or sets the width of all items in the WrapPanel.
    /// </summary>
    public double ItemWidth
    {
        get => GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    ///     Gets or sets the height of all items in the WrapPanel.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    ///     Gets the next control in the specified direction.
    /// </summary>
    /// <param name="direction">The movement direction.</param>
    /// <param name="from">The control from which movement begins.</param>
    /// <param name="wrap">Whether to wrap around when the first or last item is reached.</param>
    /// <returns>The control.</returns>
    IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
    {
        var orientation = Orientation;
        var children = Children;
        var horiz = orientation == Orientation.Horizontal;
        var index = from is not null ? Children.IndexOf((Control)from) : -1;

        switch (direction)
        {
            case NavigationDirection.First:
                index = 0;
                break;
            case NavigationDirection.Last:
                index = children.Count - 1;
                break;
            case NavigationDirection.Next:
                ++index;
                break;
            case NavigationDirection.Previous:
                --index;
                break;
            case NavigationDirection.Left:
                index = horiz ? index - 1 : -1;
                break;
            case NavigationDirection.Right:
                index = horiz ? index + 1 : -1;
                break;
            case NavigationDirection.Up:
                index = horiz ? -1 : index - 1;
                break;
            case NavigationDirection.Down:
                index = horiz ? -1 : index + 1;
                break;
        }

        if (index >= 0 && index < children.Count)
        {
            return children[index];
        }

        return null;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var orientation = Orientation;
        var children = Children;
        var curLineSize = new UVSize(orientation);
        var panelSize = new UVSize(orientation);
        var uvConstraint = new UVSize(orientation, constraint.Width, constraint.Height);
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);

        var childConstraint = new Size(
            itemWidthSet ? itemWidth : constraint.Width,
            itemHeightSet ? itemHeight : constraint.Height);

        for (int i = 0, count = children.Count; i < count; i++)
        {
            var child = children[i];
            // Flow passes its own constraint to children
            child.Measure(childConstraint);

            // This is the size of the child in UV space
            var sz = new UVSize(orientation,
                itemWidthSet ? itemWidth : child.DesiredSize.Width,
                itemHeightSet ? itemHeight : child.DesiredSize.Height);

            if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) // Need to switch to another line
            {
                panelSize.U = Max(curLineSize.U, panelSize.U);
                panelSize.V += curLineSize.V;
                curLineSize = sz;

                if (MathUtilities.GreaterThan(sz.U, uvConstraint.U)) // The element is wider then the constraint - give it a separate line
                {
                    panelSize.U = Max(sz.U, panelSize.U);
                    panelSize.V += sz.V;
                    curLineSize = new UVSize(orientation);
                }
            }
            else // Continue to accumulate a line
            {
                curLineSize.U += sz.U;
                curLineSize.V = Max(sz.V, curLineSize.V);
            }
        }

        // The last line size, if any should be added
        panelSize.U = Max(curLineSize.U, panelSize.U);
        panelSize.V += curLineSize.V;

        // Go from UV space to W/H space
        return new Size(panelSize.Width, panelSize.Height);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var orientation = Orientation;
        var children = Children;
        var firstInLine = 0;
        double accumulatedV = 0;
        var itemU = orientation == Orientation.Horizontal ? itemWidth : itemHeight;
        var curLineSize = new UVSize(orientation);
        var uvFinalSize = new UVSize(orientation, finalSize.Width, finalSize.Height);
        var itemWidthSet = !double.IsNaN(itemWidth);
        var itemHeightSet = !double.IsNaN(itemHeight);
        var useItemU = orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet;

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var sz = new UVSize(orientation,
                itemWidthSet ? itemWidth : child.DesiredSize.Width,
                itemHeightSet ? itemHeight : child.DesiredSize.Height);

            if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) // Necesita pasar a otra línea
            {
                ArrangeLine(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU);

                accumulatedV += curLineSize.V;
                curLineSize = sz;

                if (MathUtilities.GreaterThan(sz.U, uvFinalSize.U)) // El elemento es más ancho que el espacio disponible
                {
                    // Cambiar a la siguiente línea que solo contiene un elemento
                    ArrangeLine(accumulatedV, sz.V, i, i + 1, useItemU, itemU);

                    accumulatedV += sz.V;
                    curLineSize = new UVSize(orientation);
                    firstInLine = i + 1;
                }
                else
                {
                    firstInLine = i;
                }
            }
            else // Continuar acumulando en la línea actual
            {
                curLineSize.U += sz.U;
                curLineSize.V = Max(sz.V, curLineSize.V);
            }
        }

        // Disponer la última línea, si es necesario
        if (firstInLine < children.Count)
        {
            ArrangeLine(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU);
        }

        return finalSize;
    }

    private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
    {
        var orientation = Orientation;
        var children = Children;
        double u = 0;
        var isHorizontal = orientation == Orientation.Horizontal;

        for (var i = start; i < end; i++)
        {
            var child = children[i];
            var childSize = new UVSize(orientation, child.DesiredSize.Width, child.DesiredSize.Height);
            var layoutSlotU = useItemU ? itemU : childSize.U;
            child.Arrange(new Rect(
                isHorizontal ? u : v,
                isHorizontal ? v : u,
                isHorizontal ? layoutSlotU : lineV,
                isHorizontal ? lineV : layoutSlotU));
            u += layoutSlotU;
        }
    }

    private struct UVSize
    {
        internal UVSize(Orientation orientation, double width, double height)
        {
            U = V = 0d;
            _orientation = orientation;
            Width = width;
            Height = height;
        }

        internal UVSize(Orientation orientation)
        {
            U = V = 0d;
            _orientation = orientation;
        }

        internal double U;
        internal double V;
        private readonly Orientation _orientation;

        internal double Width
        {
            get => _orientation == Orientation.Horizontal ? U : V;
            set
            {
                if (_orientation == Orientation.Horizontal)
                {
                    U = value;
                }
                else
                {
                    V = value;
                }
            }
        }

        internal double Height
        {
            get => _orientation == Orientation.Horizontal ? V : U;
            set
            {
                if (_orientation == Orientation.Horizontal)
                {
                    V = value;
                }
                else
                {
                    U = value;
                }
            }
        }
    }
}