using System.Collections.Generic;
using System.Linq;
using Graphs;

namespace Zafiro.Avalonia.Graphs.Impl;

public class Graph<T> : IGraph<T>, IGraph where T : notnull
{
    private readonly Dictionary<T, Node<T>> _nodesMap;
    private readonly List<INode> _graphNodes;
    private readonly List<IEdge> _graphEdges;

    public Graph(IEnumerable<T> vertices, IList<(T, T, double)> edges)
    {
        _nodesMap = vertices.Select(x => new Node<T>(x, this)).ToDictionary(node => node.Value);
        var edgeList = edges.Select(e => new Edge<T>(e.Item1, e.Item2, e.Item3)).ToList();
        Edges = edgeList;
        _graphNodes = _nodesMap.Values.Cast<INode>().ToList();
        _graphEdges = edgeList.Cast<IEdge>().ToList();
    }

    public IEnumerable<INode<T>> Nodes => _nodesMap.Values;

    public IEnumerable<IEdge<T>> Edges { get; }

    IList<IEdge> IGraph.Edges => _graphEdges;

    IList<INode> IGraph.Nodes => _graphNodes;

    public INode<T> GetNode(T item) => _nodesMap[item];

    public INode GetNode(object o) => GetNode((T)o);
}