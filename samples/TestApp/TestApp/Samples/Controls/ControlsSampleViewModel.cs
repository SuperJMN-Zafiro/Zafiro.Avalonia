using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using ReactiveUI;
using Zafiro.Avalonia.DataViz.Dendrogram.Core;
using Zafiro.Avalonia.DataViz.Graph.Control;
using Zafiro.Avalonia.DataViz.Graph.Core;

namespace TestApp.Samples.Controls;

public class ControlsSampleViewModel
{
    public ControlsSampleViewModel()
    {
        SelectionModel = new SelectionModel<Item> { SingleSelect = false };
        Items = new List<Item> { new Item(1), new Item(2), new Item(3) };
        //SelectionHandler = new SelectionHandler<Item, int>(SelectionModel, arg => arg.Id);

        var graph = new RandomGraphGenerator().GenerateRandomGraph(30, 3);

        var edge2Ds = graph.edges.Cast<IEdge2D>();

        var graph2D = new Graph2D(graph.nodes.Cast<INode2D>().ToList(), edge2Ds.ToList());

        var engine1 = new ForceDirectedEngine(graph2D);

        engine1.Distribute(5000, 3000);
        Graph = graph2D;

        Play = ReactiveCommand
            .CreateFromObservable(() => Observable.Interval(TimeSpan.FromMilliseconds(12), RxApp.MainThreadScheduler).Do(_ => engine1.Step()));

        Play.Execute().Subscribe();

        var randomGraph = new RandomGraphGenerator().GenerateRandomGraph(30, 10);
        var graph2d = new Graph2D(randomGraph.nodes.Cast<INode2D>().ToList(), randomGraph.edges.Cast<IEdge2D>().ToList());

        var engine2 = new ForceDirectedEngine(graph2d);

        engine2.Distribute(2000, 2000);
        GradualGraph = new GradualGraph<INode2D, IEdge2D>(graph2D, new LoadOptions());

        Cluster = new Cluster(
            new Cluster(
                new Cluster(
                    new Cluster("A"), 
                    new Cluster("B"), 1),
                new Cluster("F"), 2),
            new Cluster(
                new Cluster("C"),
                new Cluster(
                    new Cluster("D"),
                    new Cluster("E"), 4), 5), 7);
    }

    public ReactiveCommand<Unit, long> Play { get; set; }

    public List<Item> Items { get; }

    //public SelectionHandler<Item, int> SelectionHandler { get; }

    public SelectionModel<Item> SelectionModel { get; }
    public IGraph2D Graph { get; }
    public GradualGraph<INode2D, IEdge2D> GradualGraph { get; }
    public Cluster Cluster { get; }
}