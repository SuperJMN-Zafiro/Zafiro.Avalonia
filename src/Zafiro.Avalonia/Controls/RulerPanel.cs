namespace Zafiro.Avalonia.Controls;

using System;

public class RulerPanel : Panel
{
    public static readonly StyledProperty<double> TickSpacingProperty =
        AvaloniaProperty.Register<RulerPanel, double>(nameof(TickSpacing), 10.0);

    public double TickSpacing
    {
        get => GetValue(TickSpacingProperty);
        set => SetValue(TickSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double maxChildHeight = 0;
        double totalWidth = 0;

        foreach (var child in Children)
        {
            // Measure each child with no constraints
            child.Measure(Size.Infinity);

            // Keep track of the maximum child height
            if (child.DesiredSize.Height > maxChildHeight)
                maxChildHeight = child.DesiredSize.Height;
        }

        // Calculate total width based on tick spacing and number of children
        if (Children.Count > 0)
        {
            totalWidth = ((Children.Count - 1) * TickSpacing) + Children[Children.Count - 1].DesiredSize.Width;
        }

        // Ensure we return a valid size (cannot be infinite)
        var desiredSize = new Size(
            double.IsInfinity(availableSize.Width) ? totalWidth : Math.Min(availableSize.Width, totalWidth),
            double.IsInfinity(availableSize.Height) ? maxChildHeight : Math.Min(availableSize.Height, maxChildHeight)
        );

        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double currentX = 0;

        foreach (var child in Children)
        {
            var childSize = child.DesiredSize;

            // Center the child at the tick's exact position
            // currentX represents the tick's center point, so we move the child left by half its width
            double centeredX = currentX - (childSize.Width / 2);

            // Vertically align it in the center of the panel
            double centeredY = (finalSize.Height - childSize.Height) / 2;

            // Arrange the child at its calculated position
            child.Arrange(new Rect(centeredX, centeredY, childSize.Width, childSize.Height));

            // Move to the next tick
            currentX += TickSpacing;
        }

        return finalSize;
    }
}
