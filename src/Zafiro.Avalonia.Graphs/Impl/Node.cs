using Graphs;

namespace Zafiro.Avalonia.Graphs.Impl;

public class Node<T> : INode<T>
{
    private IGraph _graph;
    private IGraph<T> _graphofT;

    public Node(T value, IGraph<T> graph)
    {
        Value = value;
        _graphofT = graph;
    }

    IGraph INode.Graph => (IGraph)_graphofT;
    object INode.Value => Value;

    public T Value { get; }

    IGraph<T> INode<T>.Graph => _graphofT;
}