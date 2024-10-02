using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Selection;
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

        var person = new Person("JMN") { X = 100, Y = 200 };
        var person1 = new Person("Ana") { X = 140, Y = 250 };
        var rel = new Friendship(person, person1, 33);

        var edges = new IEdge<INode2D>[] { rel };

        var nodes = new List<INode2D> { person, person1 };
        
        var graph2d = new Graph2D(nodes.ToList(), edges.ToList());
        Graph = new GraphWrapper(graph2d);
    }

    public List<Item> Items { get; }

    //public SelectionHandler<Item, int> SelectionHandler { get; }

    public SelectionModel<Item> SelectionModel { get; }
    public IGraph Graph { get; }
}

