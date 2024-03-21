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
}