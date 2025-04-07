using System;
using System.Collections;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;
using Zafiro.DataAnalysis.Graphs;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.DataViz.Graphs.Control;

public class GradualGraph<TNode, TEdge> : IGraph where TEdge : IWeightedEdge<TNode> where TNode : notnull
{
    public GradualGraph(IGraph<TNode, TEdge> inner, LoadOptions options)
    {
        Options = options;
        InnerGraph = inner;
        var vertexList = new SourceList<TNode>();
        var edgesList = new SourceList<TEdge>();

        var vertexChanges = vertexList.Connect();

        vertexChanges
            .Bind(out var nodes)
            .Subscribe();

        var edgeChanges = edgesList.Connect();

        edgeChanges
            .Bind(out var edges)
            .Subscribe();

        var edgeCount = edgeChanges.Count().StartWith(0);
        var vertexCount = vertexChanges.Count().StartWith(0);
        var currentCount = edgeCount.CombineLatest(vertexCount, (v, e) => v + e);

        var total = inner.Edges.Count() + inner.Nodes.Count();
        Progress = currentCount.Select(current => (double) current / total);

        Nodes = nodes;
        Edges = edges;

        Load = ReactiveCommand.CreateFromObservable(() => Combined(vertexList, edgesList));
    }

    public LoadOptions Options { get; }

    public IGraph<TNode,TEdge> InnerGraph { get; }
    public IObservable<double> Progress { get; }
    public ReactiveCommand<Unit, Unit> Load { get; set; }
    public IEnumerable Nodes { get; }
    public IEnumerable Edges { get; }

    private IObservable<Unit> Combined(SourceList<TNode> vertexList, SourceList<TEdge> edgesList)
    {
        vertexList.Clear();
        edgesList.Clear();
        return AddVertices(vertexList).Concat(AddEdges(edgesList));
    }

    private IObservable<Unit> AddEdges(SourceList<TEdge> edgesList)
    {
        return InnerGraph.Edges.OrderByDescending(edge => edge.Weight)
            .ToObservable()
            .Buffer(Options.EdgeBufferCount)
            .Select(list => Observable.Return(list).Delay(Options.AddDelay))
            .Merge(1)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(edgesList.AddRange)
            .ToSignal();
    }

    private IObservable<Unit> AddVertices(SourceList<TNode> vertexList)
    {
        return InnerGraph.Nodes.OrderByDescending(InnerGraph.DegreeCentrality)
            .ToObservable()
            .Buffer(Options.VertexBufferCount)
            .Select(list => Observable.Return(list).Delay(Options.AddDelay))
            .Merge(1)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(vertexList.AddRange)
            .ToSignal();
    }
}