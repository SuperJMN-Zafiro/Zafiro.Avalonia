using System.Collections.Generic;

namespace Graphs;

public interface IGraph<T>
{
    IEnumerable<INode<T>> Nodes { get; }
    IEnumerable<IEdge<T>> Edges { get; }
    INode<T> GetNode(T item);
}