namespace Zafiro.Avalonia.Controls.Panels;

public class SmartDockPanel : Panel
{
    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<SmartDockPanel, double>(nameof(VerticalSpacing));

    public static readonly StyledProperty<double> HorizontalSpacingProperty =
        AvaloniaProperty.Register<SmartDockPanel, double>(nameof(HorizontalSpacing));

    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    public double HorizontalSpacing
    {
        get => GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    // Use DockPanel's existing attached property
    public static void SetDock(Control control, Dock value) => DockPanel.SetDock(control, value);
    public static Dock GetDock(Control control) => DockPanel.GetDock(control);

    protected override Size MeasureOverride(Size availableSize)
    {
        var visibleChildren = Children.Where(c => c.IsVisible).ToList();
        if (visibleChildren.Count == 0) return new Size();

        var remainingSize = availableSize;
        var totalSize = new Size();

        // Measure docked children first
        foreach (var child in visibleChildren.Take(visibleChildren.Count - 1))
        {
            var dock = GetDock(child);
            child.Measure(remainingSize);
            var childSize = child.DesiredSize;

            switch (dock)
            {
                case Dock.Top:
                case Dock.Bottom:
                    totalSize = totalSize.WithHeight(totalSize.Height + childSize.Height);
                    totalSize = totalSize.WithWidth(Math.Max(totalSize.Width, childSize.Width));
                    remainingSize = remainingSize.WithHeight(Math.Max(0, remainingSize.Height - childSize.Height));
                    break;
                case Dock.Left:
                case Dock.Right:
                    totalSize = totalSize.WithWidth(totalSize.Width + childSize.Width);
                    totalSize = totalSize.WithHeight(Math.Max(totalSize.Height, childSize.Height));
                    remainingSize = remainingSize.WithWidth(Math.Max(0, remainingSize.Width - childSize.Width));
                    break;
            }
        }

        // Measure the last child (fills remaining space)
        if (visibleChildren.Count > 0)
        {
            var lastChild = visibleChildren.Last();
            lastChild.Measure(remainingSize);
            var lastSize = lastChild.DesiredSize;
            totalSize = new Size(
                Math.Max(totalSize.Width, lastSize.Width),
                Math.Max(totalSize.Height, lastSize.Height)
            );
        }

        // Add spacing between visible children
        if (visibleChildren.Count > 1)
        {
            var verticalSpaces = visibleChildren.Take(visibleChildren.Count - 1)
                .Count(c => GetDock(c) == Dock.Top || GetDock(c) == Dock.Bottom);
            var horizontalSpaces = visibleChildren.Take(visibleChildren.Count - 1)
                .Count(c => GetDock(c) == Dock.Left || GetDock(c) == Dock.Right);

            totalSize = totalSize.WithHeight(totalSize.Height + verticalSpaces * VerticalSpacing);
            totalSize = totalSize.WithWidth(totalSize.Width + horizontalSpaces * HorizontalSpacing);
        }

        return totalSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var visibleChildren = Children.Where(c => c.IsVisible).ToList();
        if (visibleChildren.Count == 0) return finalSize;

        var remainingRect = new Rect(finalSize);

        // Arrange docked children
        foreach (var child in visibleChildren.Take(visibleChildren.Count - 1))
        {
            var dock = GetDock(child);
            var childSize = child.DesiredSize;
            var childRect = new Rect();

            switch (dock)
            {
                case Dock.Top:
                    childRect = new Rect(remainingRect.X, remainingRect.Y, remainingRect.Width, childSize.Height);
                    remainingRect = remainingRect.WithY(remainingRect.Y + childSize.Height + VerticalSpacing)
                        .WithHeight(remainingRect.Height - childSize.Height - VerticalSpacing);
                    break;
                case Dock.Bottom:
                    childRect = new Rect(remainingRect.X, remainingRect.Bottom - childSize.Height, remainingRect.Width, childSize.Height);
                    remainingRect = remainingRect.WithHeight(remainingRect.Height - childSize.Height - VerticalSpacing);
                    break;
                case Dock.Left:
                    childRect = new Rect(remainingRect.X, remainingRect.Y, childSize.Width, remainingRect.Height);
                    remainingRect = remainingRect.WithX(remainingRect.X + childSize.Width + HorizontalSpacing)
                        .WithWidth(remainingRect.Width - childSize.Width - HorizontalSpacing);
                    break;
                case Dock.Right:
                    childRect = new Rect(remainingRect.Right - childSize.Width, remainingRect.Y, childSize.Width, remainingRect.Height);
                    remainingRect = remainingRect.WithWidth(remainingRect.Width - childSize.Width - HorizontalSpacing);
                    break;
            }

            child.Arrange(childRect);
        }

        // Arrange the last child (fills remaining space)
        if (visibleChildren.Count > 0)
        {
            var lastChild = visibleChildren.Last();
            lastChild.Arrange(remainingRect);
        }

        return finalSize;
    }
}