using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs.Core;

public interface IGenericGraph<TNode, TEdge> where TEdge : IEdge<TNode>
{
    List<TNode> Nodes { get; }
    List<TEdge> Edges { get; }
}