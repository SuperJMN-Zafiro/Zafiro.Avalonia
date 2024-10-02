using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Graphs.Core;

namespace Graphs;

public partial class Node2D<T> : ReactiveObject, INode2D<T>
{
    public Node2D(INode<T> node, IGraph2D<T> graph2D)
    {
        Node = node;
        Graph2D = graph2D;
        _graph = graph2D;
    }

    public INode<T> Node { get; }

    public IGraph<T> Graph => Node.Graph;
    object INode.Value => Value;

    public T Value => Node.Value;

    [Reactive] private double _x;
    [Reactive] private double _y;
    private IGraph _graph;

    public double ForceX { get; set; }
    public double ForceY { get; set; }
    object INode2D.Value => Value;
    public IGraph2D<T> Graph2D { get; }

    IGraph INode.Graph => _graph;
}