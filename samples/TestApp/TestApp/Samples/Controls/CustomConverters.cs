using System.Linq;
using Avalonia.Data.Converters;
using Zafiro.Avalonia.DataViz.Graph.Core;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Controls;

public static class CustomConverters
{
    public static readonly FuncMultiValueConverter<object, double> Importance = new(
        objects =>
        {
            var list = objects.ToList();
            
            if (list[0] is ControlsSampleViewModel { MutableGraph: IMutableGraph graph } && list[1] is IMutableNode node)
            {
                return graph.RelativeDegreeCentrality(node) * 50;
            }

            return 0;
        });
}