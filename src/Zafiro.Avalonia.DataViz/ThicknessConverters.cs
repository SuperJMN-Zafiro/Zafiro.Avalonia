using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.DataViz;

public static class ThicknessConverters
{
    public static FuncMultiValueConverter<object, Thickness> RowsMarginConverter { get; } = new(d =>
    {
        var list = d.ToList();
        
        if (list.Any(o => o is UnsetValueType))
        {
            return new Thickness();
        }
        
        var height = (double)list[0];
        var rows = (int)list[1];
        
        return new Thickness(0, height / rows / 2);
    });

    public static FuncMultiValueConverter<object, Thickness> ColumnsMarginConverter { get; } = new(d =>
    {
        var list = d.ToList();
        
        if (list.Any(o => o is UnsetValueType))
        {
            return new Thickness();
        }
        
        var width = (double)list[0];
        var columns = (int)list[1];
        
        return new Thickness(width / columns / 2, 0);
    });
}