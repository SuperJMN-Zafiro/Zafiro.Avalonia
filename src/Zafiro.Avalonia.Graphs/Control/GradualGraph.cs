using System;
using System.Collections;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Zafiro.Avalonia.Graphs.Core;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Graphs.Control;

public class GradualGraph<TNode, TEdge> : IGraph where TEdge : IEdge<TNode> where TNode : notnull
{
    private readonly IGenericGraph<TNode, TEdge> inner;

    public GradualGraph(IGenericGraph<TNode, TEdge> inner)
    {
        this.inner = inner;
        var vertexList = new SourceList<TNode>();
        var edgesList = new SourceList<TEdge>();

        vertexList.Connect()
            .Bind(out var nodes)
            .Subscribe();

        edgesList.Connect()
            .Bind(out var edges)
            .Subscribe();

        Nodes = nodes;
        Edges = edges;

        Load = ReactiveCommand.CreateFromObservable(() => Combined(vertexList, edgesList));
    }

    public static int EdgeBufferCount { get; set; } = 20;

    public TimeSpan AddDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    public int VertexBufferCount { get; set; } = 10;

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
        return inner.Edges.OrderByDescending(edge => edge.Weight)
            .ToObservable()
            .Buffer(EdgeBufferCount)
            .Select(list => Observable.Return(list).Delay(AddDelay))
            .Merge(1)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(edgesList.AddRange)
            .ToSignal();
    }

    private IObservable<Unit> AddVertices(SourceList<TNode> vertexList)
    {
        return inner.Nodes.OrderByDescending(inner.DegreeCentrality)
            .ToObservable()
            .Buffer(VertexBufferCount)
            .Select(list => Observable.Return(list).Delay(AddDelay))
            .Merge(1)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(vertexList.AddRange)
            .ToSignal();
    }
}