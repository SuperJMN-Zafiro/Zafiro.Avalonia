using Zafiro.Avalonia.Graphs.Core;

namespace Zafiro.Avalonia.Graphs.Tests;

public class UnitTest1
{
    [Fact]
    public void Create()
    {
        var person = new Person("JMN");
        var person1 = new Person("Ana");
        var rel = new Friendship(person, person1, 33);

        List<IEdge<INode2D>> edges = new List<IEdge<INode2D>> { rel };

        List<INode2D> nodes = new List<INode2D> { person, person1 };
        
        var graph2d = new Graph2D(nodes, edges.ToList());
        var ffd = new ForceDirectedGraph(graph2d);
        ffd.Distribute(400,400);
        ffd.Step();
    }
}