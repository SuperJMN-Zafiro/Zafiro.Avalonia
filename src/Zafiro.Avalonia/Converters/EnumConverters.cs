using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class EnumConverters
{
    public static FuncValueConverter<Type?, string[]> Names = new(type =>
    {
        if (type is { })
        {
            return Enum.GetNames(type);
        }

        return [];
    });
}