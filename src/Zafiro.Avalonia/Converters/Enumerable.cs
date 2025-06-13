using System.Collections;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public static class Enumerable
{
    public static FuncValueConverter<IEnumerable, bool> Any = new(enumerable =>
    {
        if (enumerable == null) return false;

        if (enumerable is ICollection collection)
            return collection.Count > 0;

        return enumerable.Cast<object>().Any();
    });
}