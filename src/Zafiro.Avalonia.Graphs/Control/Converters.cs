using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Control;

public class Converters
{
    public static readonly FuncMultiValueConverter<double, Point?> PointCoordinatesToPoint = new(enumerable =>
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