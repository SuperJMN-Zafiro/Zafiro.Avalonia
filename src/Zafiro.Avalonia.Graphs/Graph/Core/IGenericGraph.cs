using System.Collections.Generic;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

public interface IGenericGraph<TNode, TEdge> where TEdge : IEdge<TNode>
{
    List<TNode> Nodes { get; }
    List<TEdge> Edges { get; }
}