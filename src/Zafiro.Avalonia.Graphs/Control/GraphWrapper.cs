using System.Collections;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Control;

public class GraphWrapper : IGraph
{
    public GraphWrapper(IGraph2D graph)
    {
        Edges = graph.Edges;
        Nodes = graph.Nodes;
    }

    public IEnumerable Nodes { get; }

    public IEnumerable Edges { get; }
}
