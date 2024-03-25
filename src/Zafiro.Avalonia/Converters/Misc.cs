using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class MiscConverters
{
    public static FuncValueConverter<SelectionKind, bool?> SelectionKindToCheckBoxValue = new FuncValueConverter<SelectionKind, bool?>(kind =>
    {
        return kind switch
        {
            SelectionKind.Full => true,
            SelectionKind.Partial => null,
            SelectionKind.None => false,
        };
    });

    public static FuncValueConverter<int, string> MoreThan99 = new FuncValueConverter<int, string>(i =>
    {
        return i switch
        {
            > 99 => "+99",
            _ => i.ToString()
        };
    });
}