using System;
using Avalonia;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogSizing
{
    private const double GoldenRatio = 1.61803398875;
    private static readonly Size FallbackSize = new(1024, 768);

    public static DialogSizePlan For(Window owner)
    {
        var scaling = Maybe<double>.From(owner.PlatformImpl?.DesktopScaling).GetValueOrDefault(1d);

        var available = Maybe.From(owner.Screens?.ScreenFromVisual(owner))
            .Or(() => Maybe.From(owner.Screens?.Primary))
            .Map(screen => screen.WorkingArea)
            .Map(rect => new Size(rect.Width / scaling, rect.Height / scaling))
            .GetValueOrDefault(FallbackSize);

        return For(available);
    }

    public static DialogSizePlan For(Size available)
    {
        var marginRatio = 0.08;
        var paddingRatio = 0.05;

        var maxWidth = Math.Max(available.Width * (1 - marginRatio), 320);
        var maxHeight = Math.Max(available.Height * (1 - marginRatio), 280);

        var minWidth = Math.Min(maxWidth, Math.Max(available.Width * 0.4, 360));
        var minHeight = Math.Min(maxHeight, Math.Max(available.Height * 0.35, 280));

        var dominantWidth = available.Width >= available.Height;
        var preferredWidth = dominantWidth
            ? available.Width * 0.58
            : available.Height / GoldenRatio;

        var preferredHeight = dominantWidth
            ? preferredWidth / GoldenRatio
            : available.Height * 0.58;

        preferredWidth = Math.Clamp(preferredWidth, minWidth, maxWidth);
        preferredHeight = Math.Clamp(preferredHeight, minHeight, maxHeight);

        var minDimension = Math.Min(available.Width, available.Height);
        var padding = Math.Clamp(minDimension * paddingRatio, 16, 48);
        var outerMargin = Math.Clamp(minDimension * marginRatio, 12, 64);

        return new DialogSizePlan(minWidth, maxWidth, minHeight, maxHeight, preferredWidth, preferredHeight, padding, outerMargin);
    }

    public static void Apply(Window window, DialogSizePlan plan)
    {
        window.MaxWidth = plan.MaxWidth;
        window.MaxHeight = plan.MaxHeight;
        window.MinWidth = plan.MinWidth;
        window.MinHeight = plan.MinHeight;
        window.Width = plan.PreferredWidth;
        window.Height = plan.PreferredHeight;
        window.Padding = new Thickness(plan.Padding);
    }

    public static void Apply(DialogControl control, DialogSizePlan plan)
    {
        control.MaxWidth = plan.ContentMaxWidth;
        control.MaxHeight = plan.ContentMaxHeight;
        control.MinWidth = plan.ContentMinWidth;
        control.MinHeight = plan.ContentMinHeight;
    }

    public static void Apply(DialogViewContainer container, DialogSizePlan plan)
    {
        container.MaxWidth = plan.MaxWidth;
        container.MaxHeight = plan.MaxHeight;
        container.MinWidth = plan.MinWidth;
        container.MinHeight = plan.MinHeight;
        container.Padding = new Thickness(plan.Padding);
        container.Margin = new Thickness(plan.OuterMargin);
    }
}

public readonly record struct DialogSizePlan(
    double MinWidth,
    double MaxWidth,
    double MinHeight,
    double MaxHeight,
    double PreferredWidth,
    double PreferredHeight,
    double Padding,
    double OuterMargin)
{
    public double ContentMinWidth => Math.Max(0, MinWidth - Padding * 2);
    public double ContentMinHeight => Math.Max(0, MinHeight - Padding * 2);
    public double ContentMaxWidth => Math.Max(ContentMinWidth, MaxWidth - Padding * 2);
    public double ContentMaxHeight => Math.Max(ContentMinHeight, MaxHeight - Padding * 2);
}
