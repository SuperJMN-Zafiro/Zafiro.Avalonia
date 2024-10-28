using System;
using System.Collections;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.DataViz.Graph.Core;
using Zafiro.Graphs;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.DataViz.Graph.Control;

public class AnimatedGraph<TNode, TEdge> : ReactiveObject, IGraph where TEdge : IWeightedEdge<TNode> where TNode : notnull
{
    public AnimatedGraph(IGraph<TNode, TEdge> graph2D, IEngine engine, double width, double height, LoadOptions options,
        IScheduler? scheduler = default)
    {
        Width = width;
        Height = height;

        GradualGraph = new GradualGraph<TNode, TEdge>(graph2D, options);

        Animate = StoppableCommand.Create(
            () => Observable.Interval(SteppingInterval)
                .ObserveOn(scheduler ?? AvaloniaScheduler.Instance)
                .Do(_ => engine.Step()),
            Maybe.From(GradualGraph.Load.IsExecuting.Not()));

        Nodes = GradualGraph.Nodes;
        Edges = GradualGraph.Edges;

        Distribute = ReactiveCommand.Create(() => engine.Distribute(width, height));
        GradualGraph.Load.IsExecuting.Falses().Skip(1).InvokeCommand(Animate.StartReactive);
        Distribute.InvokeCommand(GradualGraph.Load);
        Distribute.Execute().Subscribe();
    }

    public TimeSpan SteppingInterval { get; } = TimeSpan.FromMilliseconds(60);

    public double Width { get; }
    public double Height { get; }
    public GradualGraph<TNode, TEdge> GradualGraph { get; }
    public ReactiveCommand<Unit, Unit> Distribute { get; }
    public StoppableCommand<Unit, long> Animate { get; }
    public IEnumerable Nodes { get; }
    public IEnumerable Edges { get; }
}