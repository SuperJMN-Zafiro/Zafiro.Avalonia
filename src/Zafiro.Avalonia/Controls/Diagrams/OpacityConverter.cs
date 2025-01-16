using Avalonia.Data.Converters;
using Zafiro.Avalonia.Controls.Diagrams.Enhanced;

namespace Zafiro.Avalonia.Controls.Diagrams;

public static class OpacityConverter
{
    public static FuncMultiValueConverter<object, double> Instance { get; } = new(values =>
    {
        var list = values.ToList();

        if (list.Any(o => o is UnsetValueType))
        {
            return 1d;
        }
        
        var displayMode = (LabelDisplayMode?)list[0];
        var isPointerOver = (bool?)list[1] ?? false;

        if (displayMode == LabelDisplayMode.Always)
        {
            return 1.0;
        }

        return isPointerOver ? 1d : 0d;
    });
}