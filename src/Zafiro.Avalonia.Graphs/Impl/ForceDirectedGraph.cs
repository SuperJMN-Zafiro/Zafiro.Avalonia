using System.Collections.Generic;
using System.Linq;
using Graphs;
using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Impl;

public class ForceDirectedGraph<T> : IGraph2D<T> where T : notnull
{
    private readonly Dictionary<INode<T>, INode2D<T>> _nodeToNode2D;
    private readonly Engine<T> _engine;
    private readonly List<IEdge> _graphEdges;
    private readonly List<INode> _graphNodes;

    public ForceDirectedGraph(Graph<T> graph)
    {
        Graph = graph;
        _nodeToNode2D = graph.Nodes.Select(x => (x, (INode2D<T>)new Node2D<T>(x, this)))
            .ToDictionary(x => x.x, x => x.Item2);
        Nodes = _nodeToNode2D.Values.ToList();
        Edges = graph.Edges.ToList();
        _engine = new Engine<T>(this);
        _graphEdges = Edges.Cast<IEdge>().ToList();
        _graphNodes = Nodes.Cast<INode>().ToList();
    }

    public Graph<T> Graph { get; }

    public IList<INode2D<T>> Nodes { get; }

    IList<IEdge> IGraph.Edges => _graphEdges;

    public INode GetNode(object o)
    {
        return Graph.GetNode((T)o);
    }

    IList<INode> IGraph.Nodes => _graphNodes;

    public IList<IEdge<T>> Edges { get; }

    public INode2D<T> GetNode(T item)
    {
        return _nodeToNode2D[Graph.GetNode(item)];
    }

    public void Step()
    {
        _engine.Step();
    }

    public void Distribute(int width, int height)
    {
        _engine.Distribute(width, height);
    }

    public Configuration Configuration => _engine.Configuration;

    public void AdvanceBy(int i)
    {
        for (int j = 0; j < i; j++)
        {
            _engine.Step();
        }
    }
}