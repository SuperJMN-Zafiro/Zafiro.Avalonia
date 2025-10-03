using System;
using Avalonia;

namespace Zafiro.Avalonia.Dialogs;

public record DialogLayout(
    Size PreferredWindow,
    Size MinimumWindow,
    Size MaximumWindow,
    Size PreferredContent,
    Size MinimumContent,
    Size MaximumContent);

public static class DialogSizePolicy
{
    private const double GoldenRatio = 1.6180339887498948482;
    private const double PortraitHorizontalPaddingRatio = 0.08;
    private const double PortraitVerticalPaddingRatio = 0.12;
    private const double LandscapeHorizontalPaddingRatio = 0.18;
    private const double LandscapeVerticalPaddingRatio = 0.16;
    private const double MinimumWindowWidth = 320;
    private const double MinimumWindowHeight = 240;
    private const double MinimumContentWidth = 280;
    private const double MinimumContentHeight = 220;
    private const double HorizontalChrome = 64;
    private const double VerticalChrome = 180;

    public static DialogLayout Calculate(Rect parentBounds)
    {
        if (parentBounds.Width <= 0 || parentBounds.Height <= 0)
        {
            var defaultSize = new Size(MinimumWindowWidth, MinimumWindowHeight);
            var defaultContent = new Size(MinimumContentWidth, MinimumContentHeight);
            return new DialogLayout(defaultSize, defaultSize, defaultSize, defaultContent, defaultContent, defaultContent);
        }

        var orientation = parentBounds.Width >= parentBounds.Height ? Orientation.Landscape : Orientation.Portrait;

        var horizontalPaddingRatio = orientation == Orientation.Landscape
            ? LandscapeHorizontalPaddingRatio
            : PortraitHorizontalPaddingRatio;

        var verticalPaddingRatio = orientation == Orientation.Landscape
            ? LandscapeVerticalPaddingRatio
            : PortraitVerticalPaddingRatio;

        var maxWidth = Math.Max(parentBounds.Width * (1 - horizontalPaddingRatio), MinimumWindowWidth);
        var maxHeight = Math.Max(parentBounds.Height * (1 - verticalPaddingRatio), MinimumWindowHeight);

        var preferredWidth = Math.Clamp(parentBounds.Width / GoldenRatio, MinimumWindowWidth, maxWidth);
        var preferredHeightFromWidth = preferredWidth / GoldenRatio;
        var preferredHeight = Math.Clamp(preferredHeightFromWidth, MinimumWindowHeight, maxHeight);

        if (preferredHeight >= maxHeight)
        {
            preferredHeight = maxHeight;
            preferredWidth = Math.Clamp(preferredHeight * GoldenRatio, MinimumWindowWidth, maxWidth);
        }

        var minWidth = Math.Min(preferredWidth, Math.Max(MinimumWindowWidth, maxWidth * 0.5));
        var minHeight = Math.Min(preferredHeight, Math.Max(MinimumWindowHeight, maxHeight * 0.5));

        var preferredContentWidth = Math.Max(MinimumContentWidth, preferredWidth - HorizontalChrome);
        var preferredContentHeight = Math.Max(MinimumContentHeight, preferredHeight - VerticalChrome);

        var minContentWidth = Math.Min(preferredContentWidth, Math.Max(MinimumContentWidth, minWidth - HorizontalChrome));
        var minContentHeight = Math.Min(preferredContentHeight, Math.Max(MinimumContentHeight, minHeight - VerticalChrome));

        var maxContentWidth = Math.Max(MinimumContentWidth, maxWidth - HorizontalChrome);
        var maxContentHeight = Math.Max(MinimumContentHeight, maxHeight - VerticalChrome);

        return new DialogLayout(
            new Size(preferredWidth, preferredHeight),
            new Size(minWidth, minHeight),
            new Size(maxWidth, maxHeight),
            new Size(preferredContentWidth, preferredContentHeight),
            new Size(minContentWidth, minContentHeight),
            new Size(maxContentWidth, maxContentHeight));
    }

    private enum Orientation
    {
        Portrait,
        Landscape,
    }
}
