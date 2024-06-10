using Avalonia.Data.Converters;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Converters;

public class StringConverters
{
    public static FuncValueConverter<IEnumerable<string>, string> JoinWithCommas = new(enumerable => enumerable?.JoinWithCommas() ?? "");
}