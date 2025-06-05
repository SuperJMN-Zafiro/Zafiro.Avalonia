using Avalonia.Data.Converters;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Converters;

public static class ThicknessConverters
{
    public static FuncValueConverter<double, Thickness> DoubleToThickness { get; } = new(d => new Thickness(d));

    public static FuncValueConverter<Thickness, Sides, Thickness> FilterSides { get; } = new((thickness, sides) =>
    {
        return new Thickness(
            left: sides.HasFlag(Sides.Left) ? thickness.Left : 0,
            top: sides.HasFlag(Sides.Top) ? thickness.Top : 0,
            right: sides.HasFlag(Sides.Right) ? thickness.Right : 0,
            bottom: sides.HasFlag(Sides.Bottom) ? thickness.Bottom : 0);
    });
}