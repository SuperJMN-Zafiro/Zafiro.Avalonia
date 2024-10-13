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

        var ffd = new ForceDirectedGraph(new Graph2D(graph.nodes.Cast<INode2D>().ToList(), edge2Ds.ToList()));
        ffd.Distribute(5000, 3000);
        Graph = ffd;

        Play = ReactiveCommand
            .CreateFromObservable(() => Observable.Interval(TimeSpan.FromMilliseconds(12), RxApp.MainThreadScheduler).Do(_ => ffd.Step()));

        Play.Execute().Subscribe();

        var generateRandomGraph = new RandomGraphGenerator().GenerateRandomGraph(30, 10);
        var graph2D = new ForceDirectedGraph(new Graph2D(generateRandomGraph.nodes.Cast<INode2D>().ToList(), generateRandomGraph.edges.Cast<IEdge2D>().ToList()));
        graph2D.Distribute(2000, 2000);
        GradualGraph = new GradualGraph<INode2D, IEdge2D>(graph2D, new LoadOptions());


        var cluster = new Cluster(new Cluster("A"), new Cluster("B"), 3);
        var right = new Cluster(new Cluster("C"), new Cluster("D"), 4);

        Cluster = new Cluster(cluster, right, 6);
    }

    public ReactiveCommand<Unit, long> Play { get; set; }

    public List<Item> Items { get; }

    //public SelectionHandler<Item, int> SelectionHandler { get; }

    public SelectionModel<Item> SelectionModel { get; }
    public ForceDirectedGraph Graph { get; }
    public GradualGraph<INode2D, IEdge2D> GradualGraph { get; }
    public Cluster Cluster { get; }
}