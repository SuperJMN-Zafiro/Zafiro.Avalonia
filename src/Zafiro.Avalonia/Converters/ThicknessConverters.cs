using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public static class ThicknessConverters
{
    public static FuncValueConverter<double, Thickness> DoubleToThickness { get; } = new(d => new Thickness(d));
}