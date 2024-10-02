using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs.Core;

public class Graph2D(List<INode2D> nodes, List<IEdge<INode2D>> edges) : IGraph<INode2D, IEdge<INode2D>>
{
    public List<INode2D> Nodes { get; } = nodes;
    public List<IEdge<INode2D>> Edges { get; } = edges;
}