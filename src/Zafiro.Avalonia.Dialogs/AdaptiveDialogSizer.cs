using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Zafiro.Avalonia.Dialogs;

/// <summary>
/// Provides adaptive sizing for dialog windows based on real content measurement
/// and available screen space.
/// </summary>
public static class AdaptiveDialogSizer
{
    private const double DefaultMaxWidthRatio = 0.8;
    private const double DefaultMaxHeightRatio = 0.8;
    private const double MinDialogWidth = 400;
    private const double MinDialogHeight = 250;

    /// <summary>
    /// Calculates adaptive size for a dialog based on its content and available space.
    /// </summary>
    /// <param name="content">The content control to measure.</param>
    /// <param name="availableSize">Available screen space (usually parent bounds).</param>
    /// <param name="config">Optional sizing configuration.</param>
    /// <returns>Calculated sizing result.</returns>
    public static SizingResult CalculateSize(
        Control content,
        Size availableSize,
        SizingConfig? config = null)
    {
        config ??= new SizingConfig();

        // Calculate max allowed dimensions based on available space
        var maxWidth = config.FixedWidth ?? (availableSize.Width * config.MaxWidthRatio);
        var maxHeight = config.FixedHeight ?? (availableSize.Height * config.MaxHeightRatio);

        Debug.WriteLine($"[AdaptiveDialogSizer] Available: {availableSize}, MaxWidth: {maxWidth}, MaxHeight: {maxHeight}");

        // Measure content with constrained width but infinite height
        // This allows text wrapping controls to calculate correct height
        content.Measure(new Size(maxWidth, double.PositiveInfinity));
        var desiredSize = content.DesiredSize;

        Debug.WriteLine($"[AdaptiveDialogSizer] Desired after measure: {desiredSize}");

        // Apply constraints
        var finalWidth = ConstrainDimension(
            desiredSize.Width,
            config.MinWidth,
            maxWidth,
            config.FixedWidth);

        var finalHeight = ConstrainDimension(
            desiredSize.Height,
            config.MinHeight,
            maxHeight,
            config.FixedHeight);

        var isConstrained =
            finalWidth != desiredSize.Width ||
            finalHeight != desiredSize.Height;

        Debug.WriteLine($"[AdaptiveDialogSizer] Final: {finalWidth}x{finalHeight}, Constrained: {isConstrained}");

        return new SizingResult
        {
            Width = finalWidth,
            Height = finalHeight,
            IsConstrained = isConstrained,
            DesiredSize = desiredSize,
            AvailableSize = availableSize
        };
    }

    /// <summary>
    /// Applies adaptive sizing to a window.
    /// </summary>
    /// <param name="window">The window to size.</param>
    /// <param name="content">The content to measure.</param>
    /// <param name="parentBounds">Parent window bounds.</param>
    /// <param name="config">Optional sizing configuration.</param>
    /// <returns>The sizing result.</returns>
    public static SizingResult ApplyToWindow(
        Window window,
        Control content,
        Rect parentBounds,
        SizingConfig? config = null)
    {
        var availableSize = new Size(parentBounds.Width, parentBounds.Height);
        var result = CalculateSize(content, availableSize, config);

        // Apply calculated dimensions
        window.Width = result.Width;
        window.Height = result.Height;

        // Disable SizeToContent since we're managing size explicitly
        window.SizeToContent = SizeToContent.Manual;

        // Set min/max constraints
        window.MinWidth = config?.MinWidth ?? MinDialogWidth;
        window.MinHeight = config?.MinHeight ?? MinDialogHeight;
        window.MaxWidth = result.Width;
        window.MaxHeight = result.Height;

        return result;
    }

    /// <summary>
    /// Applies adaptive sizing to a control (for adorner-based dialogs).
    /// </summary>
    /// <param name="control">The control to size.</param>
    /// <param name="content">The content to measure.</param>
    /// <param name="availableSize">Available space in the adorner layer.</param>
    /// <param name="config">Optional sizing configuration.</param>
    /// <returns>The sizing result.</returns>
    public static SizingResult ApplyToControl(
        Layoutable control,
        Control content,
        Size availableSize,
        SizingConfig? config = null)
    {
        var result = CalculateSize(content, availableSize, config);

        // Apply calculated dimensions
        control.Width = result.Width;
        control.Height = result.Height;
        control.MinWidth = config?.MinWidth ?? MinDialogWidth;
        control.MinHeight = config?.MinHeight ?? MinDialogHeight;
        control.MaxWidth = result.Width;
        control.MaxHeight = result.Height;

        // Center the control
        control.HorizontalAlignment = HorizontalAlignment.Center;
        control.VerticalAlignment = VerticalAlignment.Center;

        return result;
    }

    private static double ConstrainDimension(
        double desired,
        double min,
        double max,
        double? fixedValue)
    {
        if (fixedValue.HasValue)
        {
            return fixedValue.Value;
        }

        return Math.Max(min, Math.Min(desired, max));
    }

    /// <summary>
    /// Configuration for dialog sizing behavior.
    /// </summary>
    public record SizingConfig
    {
        /// <summary>
        /// Maximum width as a ratio of available space (0.0 - 1.0).
        /// Default is 0.8 (80%).
        /// </summary>
        public double MaxWidthRatio { get; init; } = DefaultMaxWidthRatio;

        /// <summary>
        /// Maximum height as a ratio of available space (0.0 - 1.0).
        /// Default is 0.8 (80%).
        /// </summary>
        public double MaxHeightRatio { get; init; } = DefaultMaxHeightRatio;

        /// <summary>
        /// Minimum dialog width in pixels.
        /// </summary>
        public double MinWidth { get; init; } = MinDialogWidth;

        /// <summary>
        /// Minimum dialog height in pixels.
        /// </summary>
        public double MinHeight { get; init; } = MinDialogHeight;

        /// <summary>
        /// Optional fixed width. If set, width calculation is skipped.
        /// </summary>
        public double? FixedWidth { get; init; }

        /// <summary>
        /// Optional fixed height. If set, height calculation is skipped.
        /// </summary>
        public double? FixedHeight { get; init; }
    }

    /// <summary>
    /// Result of the sizing calculation.
    /// </summary>
    public record SizingResult
    {
        public double Width { get; init; }
        public double Height { get; init; }
        public bool IsConstrained { get; init; }
        public Size DesiredSize { get; init; }
        public Size AvailableSize { get; init; }
    }
}