using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Zafiro.DataAnalysis.Graphs;

namespace Zafiro.Avalonia.DataViz.Graphs.Control;

public class Converters
{
    public static readonly FuncMultiValueConverter<double, Point?> CoordinatesToPoint = new(enumerable =>
    {
        var list = enumerable.ToList();
        return new Point(list[0], list[1]);
    });

    public static readonly FuncValueConverter<IMutableNode, int> ZIndex = new(x =>
    {
        //var importance = x?.Importance();
        //var i = (int)(importance ?? 0);
        //return i;
        return 0;
    });
}