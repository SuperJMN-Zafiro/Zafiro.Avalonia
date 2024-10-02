using System.Collections;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Control;

public class GraphWrapper : IGraph
{
    public GraphWrapper(IGraph<INode2D, IEdge<INode2D>> graph)
    {
        Edges = graph.Edges;
        Nodes = graph.Nodes;
    }

    public IEnumerable Nodes { get; }

    public IEnumerable Edges { get; }
}
