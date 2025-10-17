using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs;

/// <summary>
/// Sizing strategy that measures the actual content and adds space for dialog chrome.
/// </summary>
public class ContentMeasurementSizingStrategy : IDialogSizingStrategy
{
    private readonly double dialogChromeHeight;
    private readonly double dialogChromeWidth;

    /// <summary>
    /// Creates a new content measurement sizing strategy.
    /// </summary>
    /// <param name="dialogChromeWidth">Extra width for dialog padding, borders, etc.</param>
    /// <param name="dialogChromeHeight">Extra height for title, buttons, padding, etc.</param>
    public ContentMeasurementSizingStrategy(
        double dialogChromeWidth = 100,
        double dialogChromeHeight = 120)
    {
        this.dialogChromeWidth = dialogChromeWidth;
        this.dialogChromeHeight = dialogChromeHeight;
    }

    public Size Calculate(object content, Size availableSize, AdaptiveDialogSizer.SizingConfig config)
    {
        var maxWidth = config.FixedWidth ?? (availableSize.Width * config.MaxWidthRatio);
        var maxHeight = config.FixedHeight ?? (availableSize.Height * config.MaxHeightRatio);

        Debug.WriteLine(
            $"[ContentMeasurementStrategy] Constraints: Min={config.MinWidth}x{config.MinHeight}, Max={maxWidth}x{maxHeight}");

        // Get the actual content control (not the DialogControl wrapper)
        Control actualContent = content as Control ?? new ContentControl { Content = content };

        // Measure the actual content with max width constraint (accounting for dialog chrome)
        var contentMaxWidth = maxWidth - dialogChromeWidth;
        actualContent.Measure(new Size(contentMaxWidth, double.PositiveInfinity));
        var contentDesiredSize = actualContent.DesiredSize;

        Debug.WriteLine(
            $"[ContentMeasurementStrategy] Content desired: {contentDesiredSize}");

        // Add space for dialog chrome
        var desiredSize = new Size(
            contentDesiredSize.Width + dialogChromeWidth,
            contentDesiredSize.Height + dialogChromeHeight);

        // Apply constraints
        var finalWidth = Math.Max(config.MinWidth, Math.Min(desiredSize.Width, maxWidth));
        var finalHeight = Math.Max(config.MinHeight, Math.Min(desiredSize.Height, maxHeight));

        Debug.WriteLine(
            $"[ContentMeasurementStrategy] Final: {finalWidth}x{finalHeight}");

        return new Size(finalWidth, finalHeight);
    }
}