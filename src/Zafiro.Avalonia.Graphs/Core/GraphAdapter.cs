using System.Collections;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Graphs.Core;

public class GraphAdapter(IGraph2D graph) : IGraph
{
    public IEnumerable Nodes { get; } = graph.Nodes;

    public IEnumerable Edges { get; } = graph.Edges;

    public static readonly FuncValueConverter<IGraph2D?, IGraph?> Converter = new(graph2D => graph2D != null ? new GraphAdapter(graph2D) : null);
}