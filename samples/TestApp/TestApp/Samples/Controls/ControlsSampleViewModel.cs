using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using ReactiveUI;
using Zafiro.Avalonia.Graphs.Control;
using Zafiro.Avalonia.Graphs.Core;

namespace TestApp.Samples.Controls;

public class ControlsSampleViewModel
{
    public ControlsSampleViewModel()
    {
        SelectionModel = new SelectionModel<Item> { SingleSelect = false };
        Items = new List<Item> { new Item(1), new Item(2), new Item(3) };
        //SelectionHandler = new SelectionHandler<Item, int>(SelectionModel, arg => arg.Id);

        var jmn = new Person("JMN");
        var ana = new Person("Ana");
        var pablo = new Person("Pablo");

        var rel1 = new Friendship(jmn, ana, 1);
        var rel2 = new Friendship(jmn, pablo, 2);
        var rel3 = new Friendship(ana, pablo, 2);

        var edges = new IEdge<INode2D>[] { rel1, rel2, rel3 };
        var nodes = new List<INode2D> { jmn, ana, pablo };

        var g = new RandomGraphGenerator().GenerateRandomGraph(40, 4);
        
        var ffd = new ForceDirectedGraph(new Graph2D(g.nodes.Cast<INode2D>().ToList(), g.edges.Cast<IEdge<INode2D>>().ToList()));
        ffd.Distribute(1000, 1000);
        Graph = new GraphWrapper(ffd);

        Play = ReactiveCommand
            .CreateFromObservable(() => Observable.Interval(TimeSpan.FromMilliseconds(12), RxApp.MainThreadScheduler).Do(_ => ffd.Step()));

        Play.Execute().Subscribe();
    }

    public ReactiveCommand<Unit, long> Play { get; set; }

    public List<Item> Items { get; }

    //public SelectionHandler<Item, int> SelectionHandler { get; }

    public SelectionModel<Item> SelectionModel { get; }
    public IGraph Graph { get; }
}

public class RandomGraphGenerator
{
    private Random random = new Random();

    public (List<Person> nodes, List<Friendship> edges) GenerateRandomGraph(int maxNodes, int maxEdgesPerNode)
    {
        var nodes = GenerateNodes(maxNodes);
        var edges = GenerateEdges(nodes, maxEdgesPerNode);
        return (nodes, edges);
    }

    private List<Person> GenerateNodes(int maxNodes)
    {
        var nodes = new List<Person>();
        for (int i = 0; i < maxNodes; i++)
        {
            nodes.Add(new Person($"Person_{i}"));
        }
        return nodes;
    }

    private List<Friendship> GenerateEdges(List<Person> nodes, int maxEdgesPerNode)
    {
        var edges = new List<Friendship>();
        foreach (var node in nodes)
        {
            var edgeCount = random.Next(0, maxEdgesPerNode + 1);
            var potentialFriends = nodes.Where(n => n != node && !edges.Any(e => (e.Source== node && e.Target== n) || (e.Source== n && e.Target== node))).ToList();
            
            for (int i = 0; i < edgeCount && potentialFriends.Any(); i++)
            {
                var friend = potentialFriends[random.Next(potentialFriends.Count)];
                var strength = random.Next(1, 300); // Asumiendo que la fuerza de la amistad es entre 1 y 3
                edges.Add(new Friendship(node, friend, strength));
                potentialFriends.Remove(friend);
            }
        }
        return edges;
    }
}