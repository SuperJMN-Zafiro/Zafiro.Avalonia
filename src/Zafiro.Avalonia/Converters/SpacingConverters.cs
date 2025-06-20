using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public static class SpacingConverters
{
    public static readonly IValueConverter SpacingToLeftMargin =
        new FuncValueConverter<double, Thickness>(spacing =>
            spacing > 0 ? new Thickness(spacing, 0, 0, 0) : new Thickness(0));

    public static readonly IValueConverter SpacingToRightMargin =
        new FuncValueConverter<double, Thickness>(spacing =>
            spacing > 0 ? new Thickness(0, 0, spacing, 0) : new Thickness(0));

    public static readonly IValueConverter SpacingToTopMargin =
        new FuncValueConverter<double, Thickness>(spacing =>
            spacing > 0 ? new Thickness(0, spacing, 0, 0) : new Thickness(0));

    public static readonly IValueConverter SpacingToBottomMargin =
        new FuncValueConverter<double, Thickness>(spacing =>
            spacing > 0 ? new Thickness(0, 0, 0, spacing) : new Thickness(0));
}