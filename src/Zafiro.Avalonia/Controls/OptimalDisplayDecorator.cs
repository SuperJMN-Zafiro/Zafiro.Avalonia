using System.Diagnostics;

namespace Zafiro.Avalonia.Controls;

/// <summary>
/// A Decorator that constrains its single child to an optimal size based on the active display's client area.
/// The child will maintain a golden ratio proportion and fit within specified size boundaries relative to the display.
/// </summary>
public class OptimalDisplayDecorator : Decorator
{
    private const double GoldenRatio = 1.618033988749;

    public static readonly StyledProperty<bool> IsDebugEnabledProperty =
        AvaloniaProperty.Register<OptimalDisplayDecorator, bool>(nameof(IsDebugEnabled), false);

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

    public bool IsDebugEnabled
    {
        get => GetValue(IsDebugEnabledProperty);
        set => SetValue(IsDebugEnabledProperty, value);
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
        Log($"MEASURE start | available={availableSize}, effective=({effectiveAvailableWidth},{effectiveAvailableHeight}), display={displayBounds}");
        var optimalSize = CalculateOptimalSize(displayBounds, new Size(effectiveAvailableWidth, effectiveAvailableHeight));

        // Measure child with the calculated size
        Child.Measure(optimalSize);
        Log($"MEASURE result | optimal={optimalSize}, childDesired={Child.DesiredSize}");

        return optimalSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child == null)
        {
            return finalSize;
        }

        var displayBounds = GetActiveDisplayBounds();
        Log($"ARRANGE start | final={finalSize}, display={displayBounds}");
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
        Log($"ARRANGE result | optimal={optimalSize}, offsets=({offsetX},{offsetY})");

        return finalSize;
    }

    private Size CalculateOptimalSize(Rect displayBounds, Size parentConstraint)
    {
        var displayWidth = displayBounds.Width;
        var displayHeight = displayBounds.Height;
        Log($"CALC start | parentConstraint={parentConstraint}, display=({displayWidth}x{displayHeight})");

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
        Log($"CALC bounds | minW={minWidth:F2}, maxW={maxWidth:F2}, minH={minHeight:F2}, maxH={maxHeight:F2}");

        // Choose golden-ratio orientation based on available space (we want the minimal viable box)
        var denom = (parentConstraint.Height > 0 && double.IsFinite(parentConstraint.Height)) ? parentConstraint.Height : displayHeight;
        var numer = (parentConstraint.Width > 0 && double.IsFinite(parentConstraint.Width)) ? parentConstraint.Width : displayWidth;
        var availableRatio = numer / denom;
        var targetRatio = availableRatio > 1.0 ? GoldenRatio : 1.0 / GoldenRatio;
        Log($"CALC ratio | targetR={targetRatio:F4}");

        // Compute feasible width interval honoring ratio and constraints (ignore a single pre-measure "natural"; we will probe)
        var minWidthByRatio = Math.Max(minWidth, targetRatio * minHeight);
        var maxWidthByRatio = Math.Min(maxWidth, targetRatio * maxHeight);
        var lowerW = minWidthByRatio;
        var upperW = maxWidthByRatio;
        Log($"CALC interval | lowerW={lowerW:F2}, upperW={upperW:F2}");

        if (lowerW > upperW)
        {
            // No feasible box with the ratio; pick the closest inside the bounds
            var widthFallback = Math.Clamp(maxWidthByRatio, minWidth, maxWidth);
            var heightFallback = Math.Clamp(widthFallback / targetRatio, minHeight, maxHeight);
            var fb = new Size(widthFallback, heightFallback);
            Log($"CALC infeasible-ratio | fallback={fb}");
            return fb;
        }

        // Binary search minimal width such that the child's desired height fits within ratio-limited height (W/R) and maxHeight
        Size Probe(double w)
        {
            var heightLimit = Math.Min(maxHeight, w / targetRatio);
            Child?.Measure(new Size(w, double.PositiveInfinity));
            var desiredH = Child?.DesiredSize.Height ?? 0;
            // If desired height is NaN/Inf, treat as too big
            if (!double.IsFinite(desiredH) || desiredH < 0)
                desiredH = heightLimit + 1; // force grow
            var fits = desiredH <= heightLimit + 0.5; // small tolerance
            Log($"CALC probe | w={w:F2}, limitH={heightLimit:F2}, desiredH={desiredH:F2}, fits={fits}");
            return new Size(w, fits ? heightLimit : double.PositiveInfinity);
        }

        double lo = lowerW, hi = upperW;
        const int iters = 12;
        for (int i = 0; i < iters && hi - lo > 0.5; i++)
        {
            var mid = (lo + hi) / 2.0;
            var probe = Probe(mid);
            if (double.IsInfinity(probe.Height))
            {
                // Doesn't fit; need more width
                lo = mid;
            }
            else
            {
                // Fits; try smaller
                hi = mid;
            }
        }

        var finalW = hi; // minimal that fits
        finalW = Math.Clamp(finalW, lowerW, upperW);
        var finalH = Math.Clamp(finalW / targetRatio, minHeight, maxHeight);
        var result = new Size(finalW, finalH);
        Log($"CALC result | size={result}");
        return result;
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

    private void Log(string message)
    {
        if (IsDebugEnabled)
        {
            Debug.WriteLine($"[OptimalDisplayDecorator #{GetHashCode():X}] {message}");
        }
    }
}