namespace Zafiro.Avalonia.Controls;

/// <summary>
/// A Decorator that constrains its single child to an optimal size based on the active display's client area.
/// The child will maintain a golden ratio proportion and fit within specified size boundaries relative to the display.
/// </summary>
public class OptimalDisplayDecorator : Decorator
{
    private const double GoldenRatio = 1.618033988749;

    public static readonly StyledProperty<double> MaxProportionProperty =
        AvaloniaProperty.Register<OptimalDisplayDecorator, double>(
            nameof(MaxProportion), 0.7);

    public static readonly StyledProperty<double> MinProportionProperty =
        AvaloniaProperty.Register<OptimalDisplayDecorator, double>(
            nameof(MinProportion), 0.3);

    public static readonly StyledProperty<ContentAlignment> ContentAlignmentProperty =
        AvaloniaProperty.Register<OptimalDisplayDecorator, ContentAlignment>(
            nameof(ContentAlignment), ContentAlignment.Center);

    public OptimalDisplayDecorator()
    {
        // Ensure child visuals are clipped to the decorator's bounds
        ClipToBounds = true;
    }

    /// <summary>
    /// Maximum proportion of the display size that the content can occupy (0.0 - 1.0).
    /// Default is 0.7 (70%).
    /// </summary>
    public double MaxProportion
    {
        get => GetValue(MaxProportionProperty);
        set
        {
            var v = Math.Clamp(value, 0.0, 1.0);
            if (v < MinProportion)
            {
                // Keep invariants: Min <= Max
                MinProportion = v;
            }

            SetValue(MaxProportionProperty, v);
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Minimum proportion of the display size that the content should occupy (0.0 - 1.0).
    /// Default is 0.3 (30%).
    /// </summary>
    public double MinProportion
    {
        get => GetValue(MinProportionProperty);
        set
        {
            var v = Math.Clamp(value, 0.0, 1.0);
            if (v > MaxProportion)
            {
                // Keep invariants: Min <= Max
                MaxProportion = v;
            }

            SetValue(MinProportionProperty, v);
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Alignment of the content within the available space.
    /// </summary>
    public ContentAlignment ContentAlignment
    {
        get => GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child == null)
        {
            return new Size();
        }

        // First, constrain by available size to ensure we never exceed parent bounds
        var effectiveAvailableWidth = double.IsInfinity(availableSize.Width) ? double.MaxValue : availableSize.Width;
        var effectiveAvailableHeight = double.IsInfinity(availableSize.Height) ? double.MaxValue : availableSize.Height;

        var displayBounds = GetActiveDisplayBounds();
        var optimalSize = CalculateOptimalSize(displayBounds, new Size(effectiveAvailableWidth, effectiveAvailableHeight));

        // Measure child with the calculated size
        Child.Measure(optimalSize);

        return optimalSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child == null)
        {
            return finalSize;
        }

        var displayBounds = GetActiveDisplayBounds();
        var optimalSize = CalculateOptimalSize(displayBounds, finalSize);

        // Calculate positioning within final size
        var offsetX = 0.0;
        var offsetY = 0.0;

        if (finalSize.Width > optimalSize.Width)
        {
            var remainingWidth = finalSize.Width - optimalSize.Width;
            offsetX = ContentAlignment switch
            {
                ContentAlignment.Start => 0,
                ContentAlignment.Center => remainingWidth / 2,
                ContentAlignment.End => remainingWidth,
                _ => remainingWidth / 2
            };
        }

        if (finalSize.Height > optimalSize.Height)
        {
            var remainingHeight = finalSize.Height - optimalSize.Height;
            offsetY = ContentAlignment switch
            {
                ContentAlignment.Start => 0,
                ContentAlignment.Center => remainingHeight / 2,
                ContentAlignment.End => remainingHeight,
                _ => remainingHeight / 2
            };
        }

        Child.Arrange(new Rect(offsetX, offsetY, optimalSize.Width, optimalSize.Height));

        return finalSize;
    }

    private Size CalculateOptimalSize(Rect displayBounds, Size parentConstraint)
    {
        var displayWidth = displayBounds.Width;
        var displayHeight = displayBounds.Height;

        // Calculate min and max sizes based on proportions
        var minWidth = displayWidth * MinProportion;
        var maxWidth = displayWidth * MaxProportion;
        var minHeight = displayHeight * MinProportion;
        var maxHeight = displayHeight * MaxProportion;

        // Respect parent constraints: never exceed what parent allows
        maxWidth = Math.Min(maxWidth, parentConstraint.Width);
        maxHeight = Math.Min(maxHeight, parentConstraint.Height);
        minWidth = Math.Min(minWidth, maxWidth);
        minHeight = Math.Min(minHeight, maxHeight);

        // Determine golden ratio orientation based on available space
        // Using parent constraint to decide orientation is more reliable than premeasuring child
        var availableRatio = parentConstraint.Width / parentConstraint.Height;
        var targetRatio = availableRatio > 1.0 ? GoldenRatio : 1.0 / GoldenRatio;

        return CalculateOptimalSizeWithRatio(targetRatio, minWidth, maxWidth, minHeight, maxHeight);
    }

    private static Size CalculateOptimalSizeWithRatio(double targetRatio,
        double minWidth,
        double maxWidth,
        double minHeight,
        double maxHeight)
    {
        // Try to fit golden ratio within constraints
        double width, height;

        if (targetRatio > 1.0) // Landscape orientation
        {
            // Start with maximum width and calculate height
            width = maxWidth;
            height = width / targetRatio;

            // If height exceeds bounds, constrain by height
            if (height > maxHeight)
            {
                height = maxHeight;
                width = height * targetRatio;
            }

            // Ensure minimum constraints are met
            if (width < minWidth)
            {
                width = minWidth;
                height = width / targetRatio;
            }

            if (height < minHeight)
            {
                height = minHeight;
                width = height * targetRatio;
            }
        }
        else // Portrait orientation
        {
            // Start with maximum height and calculate width  
            height = maxHeight;
            width = height * targetRatio;

            // If width exceeds bounds, constrain by width
            if (width > maxWidth)
            {
                width = maxWidth;
                height = width / targetRatio;
            }

            // Ensure minimum constraints are met
            if (height < minHeight)
            {
                height = minHeight;
                width = height * targetRatio;
            }

            if (width < minWidth)
            {
                width = minWidth;
                height = width / targetRatio;
            }
        }

        // Final clamp to ensure we're within all bounds
        width = Math.Clamp(width, minWidth, maxWidth);
        height = Math.Clamp(height, minHeight, maxHeight);

        return new Size(width, height);
    }

    private Rect GetActiveDisplayBounds()
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var screen = topLevel.Screens.ScreenFromVisual(this) ?? topLevel.Screens.Primary;
                if (screen != null)
                {
                    // Convert from physical pixels to DIPs using render scaling
                    var scaling = topLevel.RenderScaling;
                    var wa = screen.WorkingArea;
                    var dipsRect = new Rect(
                        wa.X / scaling,
                        wa.Y / scaling,
                        wa.Width / scaling,
                        wa.Height / scaling);
                    return dipsRect;
                }
            }

            // Ultimate fallback (1920x1080 DIPs)
            return new Rect(0, 0, 1920, 1080);
        }
        catch
        {
            // Error handling: return a reasonable default
            return new Rect(0, 0, 1920, 1080);
        }
    }
}