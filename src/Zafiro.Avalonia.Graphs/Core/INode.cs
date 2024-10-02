using System.Collections.Generic;

namespace Graphs;

public interface INode<T>
{
    IGraph<T> Graph { get; }
    public T Value { get; }
}

