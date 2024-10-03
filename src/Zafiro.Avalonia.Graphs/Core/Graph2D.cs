using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs.Core;

public class Graph2D(List<INode2D> nodes, List<IEdge2D> edges) : IGraph2D
{
    public List<INode2D> Nodes { get; } = nodes;
    public List<IEdge2D> Edges { get; } = edges;
}