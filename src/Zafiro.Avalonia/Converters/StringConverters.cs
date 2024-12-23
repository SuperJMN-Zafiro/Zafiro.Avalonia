using Avalonia.Data.Converters;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Converters;

public class StringConverters
{
    public static FuncValueConverter<IEnumerable<string>, string> JoinWithCommas = new(enumerable => enumerable?.JoinWithCommas() ?? "");
    public static FuncMultiValueConverter<string, string> Concat { get; } = new(string.Concat);
    public static FuncMultiValueConverter<string, string> JoinWithSpaces { get; } = new(strings => { return strings.Select(s => s ?? "").JoinWith(" "); });

    public static FuncMultiValueConverter<object, string> String { get; } = new(obj => obj.FirstOrDefault()?.ToString() ?? string.Empty);
};