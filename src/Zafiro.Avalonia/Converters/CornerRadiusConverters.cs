using Avalonia.Controls.Converters;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public static class CornerRadiusConverters
{
    public static FuncValueConverter<CornerRadius, Corners, CornerRadius> FilterCorners { get; } = new((thickness, sides) =>
    {
        return new CornerRadius(
            topLeft: sides.HasFlag(Corners.TopLeft) ? thickness.TopLeft : 0,
            topRight: sides.HasFlag(Corners.TopRight) ? thickness.TopRight : 0,
            bottomRight: sides.HasFlag(Corners.BottomLeft) ? thickness.BottomLeft : 0,
            bottomLeft: sides.HasFlag(Corners.BottomRight) ? thickness.BottomRight : 0);
    });

    public static FuncValueConverter<CornerRadius, CornerRadius> TopCornerRadius { get; } = new(cornerRadius => new CornerRadius(topLeft: cornerRadius.TopLeft, topRight: cornerRadius.TopRight, bottomRight: 0, bottomLeft: 0));
    public static FuncValueConverter<CornerRadius, CornerRadius> BottomCornerRadius { get; } = new(cornerRadius => new CornerRadius(topLeft: 0, topRight: 0, bottomRight: cornerRadius.BottomLeft, bottomLeft: cornerRadius.BottomRight));
}