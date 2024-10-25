using System;
using System.Collections;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.DataViz.Graph.Core;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.DataViz.Graph.Control;

public class AnimatedGraph : ReactiveObject, IGraph
{
    public AnimatedGraph(IGraph2D graph2D, double width, double height, LoadOptions options,
        IScheduler? scheduler = default)
    {
        Graph2D = graph2D;
        Width = width;
        Height = height;

        GradualGraph = new GradualGraph<INode2D, IEdge2D>(graph2D, options);

        var forceDirectedGraph = new ForceDirectedGraph(graph2D);

        Animate = StoppableCommand.Create(
            () => Observable.Interval(SteppingInterval)
                .ObserveOn(scheduler ?? AvaloniaScheduler.Instance)
                .Do(_ => forceDirectedGraph.Step()),
            Maybe.From(GradualGraph.Load.IsExecuting.Not()));

        Nodes = GradualGraph.Nodes;
        Edges = GradualGraph.Edges;

        Distribute = ReactiveCommand.Create(() => forceDirectedGraph.Distribute(width, height));
        GradualGraph.Load.IsExecuting.Falses().Skip(1).InvokeCommand(Animate.StartReactive);
        Distribute.InvokeCommand(GradualGraph.Load);
        Distribute.Execute().Subscribe();
    }

    public TimeSpan SteppingInterval { get; } = TimeSpan.FromMilliseconds(60);

    public IGraph2D Graph2D { get; }
    public double Width { get; }
    public double Height { get; }
    public GradualGraph<INode2D, IEdge2D> GradualGraph { get; }
    public ReactiveCommand<Unit, Unit> Distribute { get; }
    public StoppableCommand<Unit, long> Animate { get; }
    public IEnumerable Nodes { get; }
    public IEnumerable Edges { get; }
}