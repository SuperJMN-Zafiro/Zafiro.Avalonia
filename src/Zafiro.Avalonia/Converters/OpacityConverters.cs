using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public static class OpacityConverters
{
    public static readonly IValueConverter BoolToOpacity =
        new FuncValueConverter<bool, double>(x => x ? 1.0 : 0.0);

    public static readonly IValueConverter BoolToOpacityInverted =
        new FuncValueConverter<bool, double>(x => x ? 0.0 : 1.0);

    public static readonly IValueConverter OpacityToBool =
        new FuncValueConverter<double, bool>(x => x > 0);
}