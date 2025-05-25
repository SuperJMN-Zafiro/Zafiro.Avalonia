using System.Linq;
using Avalonia.Data.Converters;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Misc;

public static class GraphConverters
{
    public static readonly FuncMultiValueConverter<object, double> Importance = new(objects =>
    {
        var list = objects.ToList();

        if (list[0] is GradualGraphViewModel { MutableGraph: IMutableGraph graph } && list[1] is IMutableNode node)
        {
            return graph.RelativeDegreeCentrality(node) * 50;
        }

        return 0;
    });
}