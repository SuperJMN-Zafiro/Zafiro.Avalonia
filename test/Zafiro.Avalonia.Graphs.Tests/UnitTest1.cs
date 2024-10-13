using Zafiro.Avalonia.DataViz.Graph.Core;

namespace Zafiro.Avalonia.Graphs.Tests;

public class UnitTest1
{
    [Fact]
    public void Create()
    {
        var person = new Person("One");
        var person1 = new Person("Two");
        var rel = new Friendship(person, person1, 33);

        var edges = new List<IEdge2D> { rel };
        var nodes = new List<INode2D> { person, person1 };
        
        var graph2d = new Graph2D(nodes, edges.ToList());
        var ffd = new ForceDirectedGraph(graph2d);
        ffd.Distribute(400,400);
        ffd.Step();
    }
}