using System.Collections;

namespace Zafiro.Avalonia.Graphs.Core;

public class GraphAdapter : IGraph
{
    public GraphAdapter(IGraph2D graph)
    {
        Edges = graph.Edges;
        Nodes = graph.Nodes;
    }

    public IEnumerable Nodes { get; }

    public IEnumerable Edges { get; }
}
