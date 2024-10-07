using System.Linq;
using Avalonia.Data.Converters;
using Zafiro.Avalonia.Graphs.Core;

namespace TestApp.Samples.Controls;

public static class CustomConverters
{
    public static readonly FuncMultiValueConverter<object, double> Importance = new(
        objects =>
        {
            var list = objects.ToList();
            
            if (list[0] is ControlsSampleViewModel { Graph: IGraph2D graph } && list[1] is INode2D node)
            {
                return graph.RelativeDegreeCentrality(node) * 50;
            }

            return double.NaN;
        });
}