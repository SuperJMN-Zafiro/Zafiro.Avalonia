using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Zafiro.Avalonia.DataViz.Graph.Core;

namespace Zafiro.Avalonia.DataViz.Graph.Control;

public class Converters
{
    public static readonly FuncMultiValueConverter<double, Point?> CoordinatesToPoint = new(enumerable =>
    {
        var list = enumerable.ToList();
        return new Point(list[0], list[1]);
    });

    public static readonly FuncValueConverter<INode2D, int> ZIndex = new(x =>
    {
        //var importance = x?.Importance();
        //var i = (int)(importance ?? 0);
        //return i;
        return 0;
    });
}