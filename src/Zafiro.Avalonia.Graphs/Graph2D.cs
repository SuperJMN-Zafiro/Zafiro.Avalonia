using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs;

public class Graph2D<TNode2D, TEdge>(List<TNode2D> nodes, List<TEdge> edges) where TNode2D : INode2D where TEdge : IEdge<TNode2D>
{
    public List<TNode2D> Nodes { get; } = nodes;
    public List<TEdge> Edges { get; } = edges;
}