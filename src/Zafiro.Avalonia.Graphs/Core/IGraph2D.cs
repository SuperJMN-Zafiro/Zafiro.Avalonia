using System.Collections.Generic;
using Graphs;

namespace Zafiro.Avalonia.Graphs.Core;

public interface IGraph2D<T> : IGraph<T>
{
    public IList<INode2D<T>> Nodes2d { get; }
    public INode2D<T> GetNode(T item);
}