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

        Friendship[] edges = new[] {rel};

        var nodes = new List<Person> { person, person1 };
        
        var graph2d = new Graph2D<Person, Friendship>(nodes, edges.ToList());
        var ffd = new ForceDirectedGraph<Person, Friendship>(graph2d);
        ffd.Distribute();
        ffd.Step();
    }
}