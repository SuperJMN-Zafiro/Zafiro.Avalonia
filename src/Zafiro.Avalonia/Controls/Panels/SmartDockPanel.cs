using System.Linq;

namespace Zafiro.Avalonia.Controls.Panels;

/// <summary>
/// A panel which arranges its children at the top, bottom, left, right or center.
/// This version behaves like <see cref="DockPanel"/> but ignores invisible children
/// when measuring and arranging.
/// </summary>
public class SmartDockPanel : Panel
{
    /// <summary>
    /// Defines the <see cref="LastChildFill"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> LastChildFillProperty =
        AvaloniaProperty.Register<SmartDockPanel, bool>(
            nameof(LastChildFill),
            defaultValue: true);

    /// <summary>
    /// Identifies the HorizontalSpacing dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="HorizontalSpacing"/> dependency property.</returns>
    public static readonly StyledProperty<double> HorizontalSpacingProperty =
        AvaloniaProperty.Register<SmartDockPanel, double>(
            nameof(HorizontalSpacing));

    /// <summary>
    /// Identifies the VerticalSpacing dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="VerticalSpacing"/> dependency property.</returns>
    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<SmartDockPanel, double>(
            nameof(VerticalSpacing));

    static SmartDockPanel()
    {
        AffectsParentMeasure<SmartDockPanel>(DockPanel.DockProperty);
        AffectsMeasure<SmartDockPanel>(LastChildFillProperty, HorizontalSpacingProperty, VerticalSpacingProperty);
    }

    /// <summary>
    /// Gets or sets a value which indicates whether the last child of the
    /// <see cref="SmartDockPanel"/> fills the remaining space in the panel.
    /// </summary>
    public bool LastChildFill
    {
        get => GetValue(LastChildFillProperty);
        set => SetValue(LastChildFillProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal distance between the child objects.
    /// </summary>
    public double HorizontalSpacing
    {
        get => GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical distance between the child objects.
    /// </summary>
    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var parentWidth = 0d;
        var parentHeight = 0d;
        var accumulatedWidth = 0d;
        var accumulatedHeight = 0d;

        var horizontalSpacing = false;
        var verticalSpacing = false;
        var visibleChildren = Children.Where(x => x.IsVisible).ToList();
        var childrenCount = LastChildFill ? visibleChildren.Count - 1 : visibleChildren.Count;

        for (var index = 0; index < childrenCount; ++index)
        {
            var child = visibleChildren[index];
            var childConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;

            switch (DockPanel.GetDock(child))
            {
                case Dock.Left:
                case Dock.Right:
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    if (horizontalSpacing)
                        accumulatedWidth += HorizontalSpacing;
                    accumulatedWidth += childDesiredSize.Width;
                    horizontalSpacing = true;
                    break;

                case Dock.Top:
                case Dock.Bottom:
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    if (verticalSpacing)
                        accumulatedHeight += VerticalSpacing;
                    accumulatedHeight += childDesiredSize.Height;
                    verticalSpacing = true;
                    break;
            }
        }

        if (LastChildFill && visibleChildren.Count > 0)
        {
            var child = visibleChildren[^1];
            var childConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;
            parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
            parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
            accumulatedHeight += childDesiredSize.Height;
            accumulatedWidth += childDesiredSize.Width;
        }
        else
        {
            if (horizontalSpacing)
                accumulatedWidth -= HorizontalSpacing;
            if (verticalSpacing)
                accumulatedHeight -= VerticalSpacing;
        }

        parentWidth = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);
        return new Size(parentWidth, parentHeight);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var visibleChildren = Children.Where(x => x.IsVisible).ToList();
        if (visibleChildren.Count is 0)
            return finalSize;

        var currentBounds = new Rect(finalSize);
        var childrenCount = LastChildFill ? visibleChildren.Count - 1 : visibleChildren.Count;

        for (var index = 0; index < childrenCount; ++index)
        {
            var child = visibleChildren[index];
            var dock = DockPanel.GetDock(child);
            double width, height;

            switch (dock)
            {
                case Dock.Left:
                    width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                    child.Arrange(currentBounds.WithWidth(width));
                    width += HorizontalSpacing;
                    currentBounds = new Rect(currentBounds.X + width, currentBounds.Y, Math.Max(0, currentBounds.Width - width), currentBounds.Height);
                    break;

                case Dock.Top:
                    height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                    child.Arrange(currentBounds.WithHeight(height));
                    height += VerticalSpacing;
                    currentBounds = new Rect(currentBounds.X, currentBounds.Y + height, currentBounds.Width, Math.Max(0, currentBounds.Height - height));
                    break;

                case Dock.Right:
                    width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                    child.Arrange(new Rect(currentBounds.X + currentBounds.Width - width, currentBounds.Y, width, currentBounds.Height));
                    width += HorizontalSpacing;
                    currentBounds = currentBounds.WithWidth(Math.Max(0, currentBounds.Width - width));
                    break;

                case Dock.Bottom:
                    height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                    child.Arrange(new Rect(currentBounds.X, currentBounds.Y + currentBounds.Height - height, currentBounds.Width, height));
                    height += VerticalSpacing;
                    currentBounds = currentBounds.WithHeight(Math.Max(0, currentBounds.Height - height));
                    break;
            }
        }

        if (LastChildFill && visibleChildren.Count > 0)
        {
            var child = visibleChildren[^1];
            child.Arrange(new Rect(currentBounds.X, currentBounds.Y, currentBounds.Width, currentBounds.Height));
        }

        return finalSize;
    }
}

