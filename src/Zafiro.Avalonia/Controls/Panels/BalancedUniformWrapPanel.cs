namespace Zafiro.Avalonia.Controls.Panels;

public class BalancedUniformWrapPanel : Panel
{
    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(MinItemWidth), 0d);

    public static readonly StyledProperty<double> MinItemHeightProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(MinItemHeight), 0d);

    public static readonly StyledProperty<double> MaxItemWidthProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(MaxItemWidth), double.PositiveInfinity);

    public static readonly StyledProperty<double> MaxItemHeightProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(MaxItemHeight), double.PositiveInfinity);

    public static readonly StyledProperty<double> HorizontalSpacingProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(HorizontalSpacing), 0d);

    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<BalancedUniformWrapPanel, double>(nameof(VerticalSpacing), 0d);

    /// <summary>
    /// Minimum width for each item (strict).
    /// </summary>
    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    /// <summary>
    /// Minimum height for each item (strict).
    /// </summary>
    public double MinItemHeight
    {
        get => GetValue(MinItemHeightProperty);
        set => SetValue(MinItemHeightProperty, value);
    }

    /// <summary>
    /// Maximum width for each item (optional, Infinity by default).
    /// </summary>
    public double MaxItemWidth
    {
        get => GetValue(MaxItemWidthProperty);
        set => SetValue(MaxItemWidthProperty, value);
    }

    /// <summary>
    /// Maximum height for each item (optional, Infinity by default).
    /// </summary>
    public double MaxItemHeight
    {
        get => GetValue(MaxItemHeightProperty);
        set => SetValue(MaxItemHeightProperty, value);
    }

    /// <summary>
    /// Horizontal spacing between items (in pixels).
    /// </summary>
    public double HorizontalSpacing
    {
        get => GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Vertical spacing between rows (in pixels).
    /// </summary>
    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var children = GetVisibleChildren();
        int n = children.Count;
        if (n == 0)
        {
            return new Size(0, 0);
        }

        double minW = Math.Max(0, MinItemWidth);
        double minH = Math.Max(0, MinItemHeight);
        double maxW = MaxItemWidth;
        double maxH = MaxItemHeight;
        double hSpacing = Math.Max(0, HorizontalSpacing);
        double vSpacing = Math.Max(0, VerticalSpacing);

        // Determine column count (c) and item width (iw)
        int columns;
        double itemWidth;

        if (double.IsInfinity(availableSize.Width))
        {
            // Measure children to discover their natural desired width, then clamp to min/max.
            double maxDesiredW = 0;
            foreach (var child in children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                maxDesiredW = Math.Max(maxDesiredW, child.DesiredSize.Width);
            }

            itemWidth = Clamp(maxDesiredW, minW, maxW);
            columns = n; // put everything in one balanced row when width is unconstrained
        }
        else
        {
            double availableW = Math.Max(0, availableSize.Width);

            // Solve columns from constraints with spacing:
            // columns * minW + (columns - 1) * hSpacing <= availableW
            // => columns <= floor((availableW + hSpacing) / (minW + hSpacing))
            int cMaxByMin = (int)Math.Floor((availableW + hSpacing) / Math.Max(1e-6, (minW + hSpacing)));
            cMaxByMin = Math.Min(Math.Max(1, cMaxByMin), n);

            // If maxW is finite:
            // columns * maxW + (columns - 1) * hSpacing >= availableW
            // => columns >= ceil((availableW + hSpacing) / (maxW + hSpacing))
            int cMinByMax = 1;
            if (!double.IsInfinity(maxW) && maxW > 0)
            {
                cMinByMax = (int)Math.Ceiling((availableW + hSpacing) / (maxW + hSpacing));
                cMinByMax = Math.Min(Math.Max(1, cMinByMax), n);
            }

            int low = Math.Min(cMinByMax, cMaxByMin);
            int high = Math.Max(cMinByMax, cMaxByMin);

            columns = LargestDivisorInRange(n, low, high);

            if (columns == 0)
            {
                columns = LargestDivisorInRange(n, 1, high);
                if (columns == 0)
                {
                    columns = 1;
                }
            }

            // Distribute remaining space including spacing between columns
            itemWidth = (availableW - (columns - 1) * hSpacing) / columns;
            itemWidth = Clamp(itemWidth, minW, maxW);
        }

        // Measure children with fixed item width to compute uniform height.
        double maxDesiredH = 0;
        foreach (var child in children)
        {
            child.Measure(new Size(itemWidth, double.PositiveInfinity));
            maxDesiredH = Math.Max(maxDesiredH, child.DesiredSize.Height);
        }

        double itemHeight = Clamp(maxDesiredH, minH, maxH);
        int rows = (int)Math.Ceiling((double)n / columns);

        double desiredW = double.IsInfinity(availableSize.Width)
            ? columns * itemWidth + (columns - 1) * hSpacing
            : availableSize.Width;

        double desiredH = rows * itemHeight + (rows - 1) * vSpacing;
        return new Size(desiredW, desiredH);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = GetVisibleChildren();
        int n = children.Count;
        if (n == 0)
        {
            return finalSize;
        }

        double minW = Math.Max(0, MinItemWidth);
        double minH = Math.Max(0, MinItemHeight);
        double maxW = MaxItemWidth;
        double maxH = MaxItemHeight;
        double hSpacing = Math.Max(0, HorizontalSpacing);
        double vSpacing = Math.Max(0, VerticalSpacing);

        double availableW = Math.Max(0, finalSize.Width);

        // Recompute columns similarly to Measure to keep consistency.
        int columns;
        double itemWidth;

        if (double.IsInfinity(availableW))
        {
            // Shouldn't typically happen in Arrange, but handle gracefully.
            double maxDesiredW = 0;
            foreach (var child in children)
            {
                maxDesiredW = Math.Max(maxDesiredW, child.DesiredSize.Width);
            }

            itemWidth = Clamp(maxDesiredW, minW, maxW);
            columns = n;
        }
        else
        {
            int cMaxByMin = (int)Math.Floor((availableW + hSpacing) / Math.Max(1e-6, (minW + hSpacing)));
            cMaxByMin = Math.Min(Math.Max(1, cMaxByMin), n);

            int cMinByMax = 1;
            if (!double.IsInfinity(maxW) && maxW > 0)
            {
                cMinByMax = (int)Math.Ceiling((availableW + hSpacing) / (maxW + hSpacing));
                cMinByMax = Math.Min(Math.Max(1, cMinByMax), n);
            }

            int low = Math.Min(cMinByMax, cMaxByMin);
            int high = Math.Max(cMinByMax, cMaxByMin);

            columns = LargestDivisorInRange(n, low, high);
            if (columns == 0)
            {
                columns = LargestDivisorInRange(n, 1, high);
                if (columns == 0) columns = 1;
            }

            itemWidth = Clamp((availableW - (columns - 1) * hSpacing) / columns, minW, maxW);
        }

        // Determine a uniform height consistent with the measured DesiredSize.
        double maxDesiredH = 0;
        foreach (var child in children)
        {
            maxDesiredH = Math.Max(maxDesiredH, child.DesiredSize.Height);
        }

        double itemHeight = Clamp(maxDesiredH, minH, maxH);

        int rows = (int)Math.Ceiling((double)n / columns);

        double x = 0, y = 0;
        int col = 0;
        for (int i = 0; i < n; i++)
        {
            var child = children[i];
            child.Arrange(new Rect(x, y, itemWidth, itemHeight));

            col++;
            if (col >= columns)
            {
                col = 0;
                x = 0;
                y += itemHeight + vSpacing;
            }
            else
            {
                x += itemWidth + hSpacing;
            }
        }

        // Panel consumes the provided final size in width, and the computed rows*itemHeight plus spacings in height.
        return new Size(finalSize.Width, rows * itemHeight + (rows - 1) * vSpacing);
    }

    private static int LargestDivisorInRange(int n, int fromInclusive, int toInclusive)
    {
        if (n <= 0 || fromInclusive > toInclusive) return 0;
        fromInclusive = Math.Max(1, fromInclusive);
        toInclusive = Math.Max(fromInclusive, toInclusive);

        for (int c = Math.Min(n, toInclusive); c >= fromInclusive; c--)
        {
            if (n % c == 0) return c;
        }

        return 0;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (double.IsNaN(value)) return min;
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private List<Control> GetVisibleChildren()
    {
        // Avalonia Panel.Children can contain non-visible items; layout ignores collapsed ones by convention in many panels.
        // Here we filter by IsVisible to avoid reserving space for invisible children.
        var list = new List<Control>();
        foreach (var child in Children)
        {
            if (child.IsVisible)
            {
                list.Add(child);
            }
        }

        return list;
    }
}