using System.Collections;
using Org.W3c.Dom;

namespace Zafiro.Avalonia.Graphs.Tests.Core;

public class MyGraph
{
    public MyGraph(IGraph<INode2D, IEdge<INode2D>> graph)
    {
        Edges = graph.Edges;
        Nodes = graph.Nodes;
    }

    public IEnumerable Nodes { get; }

    public IEnumerable Edges { get; }
}

public class Graph2D<TNode2D, TEdge>(List<TNode2D> nodes, List<TEdge> edges) where TNode2D : INode2D where TEdge : IEdge<TNode2D>
{
    public List<TNode2D> Nodes { get; } = nodes;
    public List<TEdge> Edges { get; } = edges;
}