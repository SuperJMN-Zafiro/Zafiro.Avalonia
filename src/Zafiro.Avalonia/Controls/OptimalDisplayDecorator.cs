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

    private Size lastOptimalSize;

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

        // Constrain by available size to ensure we never exceed parent bounds
        var effectiveAvailableWidth = double.IsInfinity(availableSize.Width) ? double.MaxValue : availableSize.Width;
        var effectiveAvailableHeight = double.IsInfinity(availableSize.Height) ? double.MaxValue : availableSize.Height;
        var parentConstraint = new Size(effectiveAvailableWidth, effectiveAvailableHeight);

        var displayBounds = GetActiveDisplayBounds();
        Log(() => $"MEASURE start | available={availableSize}, effective=({effectiveAvailableWidth},{effectiveAvailableHeight}), display={displayBounds}");

        // Compute bounds and ratio once
        var (minW, maxW, minH, maxH, ratio) = GetSizingParameters(displayBounds, parentConstraint);

        // Ratio-constrained width interval
        var lowerW = Math.Max(minW, ratio * minH);
        var upperW = Math.Min(maxW, ratio * maxH);
        Log(() => $"CALC interval | lowerW={lowerW:F2}, upperW={upperW:F2}");

        Size Probe(double w)
        {
            var heightLimit = Math.Min(maxH, w / ratio);
            Child.Measure(new Size(w, double.PositiveInfinity));
            var desiredH = Child.DesiredSize.Height;
            if (!double.IsFinite(desiredH) || desiredH < 0) desiredH = heightLimit + 1;
            var fits = desiredH <= heightLimit + 0.5;
            Log(() => $"CALC probe | w={w:F2}, limitH={heightLimit:F2}, desiredH={desiredH:F2}, fits={fits}");
            return new Size(w, fits ? heightLimit : double.PositiveInfinity);
        }

        Size optimalSize;
        if (lowerW > upperW)
        {
            var widthFallback = Math.Clamp(upperW, minW, maxW);
            var heightFallback = Math.Clamp(widthFallback / ratio, minH, maxH);
            optimalSize = new Size(widthFallback, heightFallback);
            Log(() => $"CALC infeasible-ratio | fallback={optimalSize}");
        }
        else
        {
            // Early check at the lower bound
            var probe = Probe(lowerW);
            double w;
            if (!double.IsInfinity(probe.Height))
            {
                w = lowerW;
            }
            else
            {
                double lo = lowerW, hi = upperW;
                const int iters = 5; // reduced iterations for performance
                for (int i = 0; i < iters && hi - lo > 0.5; i++)
                {
                    var mid = (lo + hi) / 2.0;
                    var p = Probe(mid);
                    if (double.IsInfinity(p.Height))
                        lo = mid; // too small
                    else
                        hi = mid; // fits; try smaller
                }

                w = hi;
            }

            var h = Math.Clamp(w / ratio, minH, maxH);
            optimalSize = new Size(w, h);
        }

        // Final measure with the computed optimal constraint
        Child.Measure(optimalSize);
        lastOptimalSize = optimalSize;

        Log(() => $"MEASURE result | optimal={optimalSize}, childDesired={Child.DesiredSize}");
        return optimalSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child == null)
        {
            return finalSize;
        }

        var displayBounds = GetActiveDisplayBounds();
        Log(() => $"ARRANGE start | final={finalSize}, display={displayBounds}");

        // Use the last measured optimal size; scale down if parent gives smaller final size
        var optimalSize = lastOptimalSize;
        if (finalSize.Width < optimalSize.Width || finalSize.Height < optimalSize.Height)
        {
            var scale = Math.Min(finalSize.Width / optimalSize.Width, finalSize.Height / optimalSize.Height);
            scale = double.IsFinite(scale) ? Math.Min(1.0, scale) : 1.0;
            optimalSize = new Size(optimalSize.Width * scale, optimalSize.Height * scale);
        }

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
        Log(() => $"ARRANGE result | optimal={optimalSize}, offsets=({offsetX},{offsetY})");

        return finalSize;
    }

    // Compute sizing parameters once (bounds + target ratio) from display and parent constraints
    private (double minW, double maxW, double minH, double maxH, double ratio) GetSizingParameters(Rect displayBounds, Size parentConstraint)
    {
        var displayWidth = displayBounds.Width;
        var displayHeight = displayBounds.Height;
        Log(() => $"CALC start | parentConstraint={parentConstraint}, display=({displayWidth}x{displayHeight})");

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
        Log(() => $"CALC bounds | minW={minWidth:F2}, maxW={maxWidth:F2}, minH={minHeight:F2}, maxH={maxHeight:F2}");

        // Choose golden-ratio orientation based on available space (we want the minimal viable box)
        var denom = (parentConstraint.Height > 0 && double.IsFinite(parentConstraint.Height)) ? parentConstraint.Height : displayHeight;
        var numer = (parentConstraint.Width > 0 && double.IsFinite(parentConstraint.Width)) ? parentConstraint.Width : displayWidth;
        var availableRatio = numer / denom;
        var targetRatio = availableRatio > 1.0 ? GoldenRatio : 1.0 / GoldenRatio;
        Log(() => $"CALC ratio | targetR={targetRatio:F4}");
        return (minWidth, maxWidth, minHeight, maxHeight, targetRatio);
    }

    // Solve for the minimal ratio-preserving box that contains 'desired' and fits within bounds
    private static Size SolveMinimalRatioBox(double targetRatio,
        double minWidth,
        double maxWidth,
        double minHeight,
        double maxHeight,
        Size desired)
    {
        // Compute feasible h interval so that w = h * R
        var lowerH = Math.Max(Math.Max(minHeight, desired.Height), Math.Max(desired.Width / targetRatio, minWidth / targetRatio));
        var upperH = Math.Min(maxHeight, maxWidth / targetRatio);

        if (lowerH > upperH)
        {
            // Infeasible exact fit; clamp to closest within bounds
            var h = Math.Clamp(lowerH, minHeight, upperH);
            var w = targetRatio * h;
            w = Math.Clamp(w, minWidth, maxWidth);
            return new Size(w, h);
        }

        var finalH = lowerH;
        var finalW = targetRatio * finalH;

        // Final clamp to ensure we're within all bounds
        finalW = Math.Clamp(finalW, minWidth, maxWidth);
        finalH = Math.Clamp(finalH, minHeight, maxHeight);

        return new Size(finalW, finalH);
    }

    private static bool AreClose(Size a, Size b)
    {
        const double eps = 0.5; // small tolerance in DIPs
        return Math.Abs(a.Width - b.Width) <= eps && Math.Abs(a.Height - b.Height) <= eps;
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
                var screens = topLevel.Screens;
                var screen = screens?.ScreenFromVisual(this) ?? screens?.Primary;
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

    private void Log(Func<string> messageFactory)
    {
        if (!IsDebugEnabled)
        {
            return;
        }

        Debug.WriteLine($"[OptimalDisplayDecorator #{GetHashCode():X}] {messageFactory()}");
    }
}