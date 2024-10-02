using System.Linq;
using Zafiro.Avalonia.Graphs.Tests;
using Zafiro.Avalonia.Graphs.Tests.Core;

namespace Zafiro.Avalonia.Graphs.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var person = new Person("JMN");
        var person1 = new Person("Ana");
        var rel = new Friendship(person, person1, 33);

        Friendship[] edges = new[] {rel};

        var nodes = new List<Person> { person, person1 };
        

        var personNodes = nodes.Select(x => new PersonNode(x)).ToList();
        var dict = personNodes.ToDictionary(node => node.Person);
        var personEdges = edges.Select(x => new PersonEdge(dict[x.Source], dict[x.Target], 20)).ToList();

        var graph2d = new Graph2D<PersonNode, PersonEdge>(personNodes, personEdges);
        var ffd = new FCC<PersonNode, PersonEdge>(graph2d);
        ffd.Distribute();
        ffd.Step();
    }
}

public class FCC<TNode2D, TEdge> where TNode2D : class, INode2D where TEdge : IEdge<TNode2D>
{
    public Graph2D<TNode2D, TEdge> Graph2d { get; }
    private readonly Engine<TNode2D, TEdge> engine;

    public FCC(Graph2D<TNode2D, TEdge> graph2d)
    {
        Graph2d = graph2d;
        engine = new Engine<TNode2D, TEdge>(graph2d);
    }

    public void Step()
    {
        engine.Step();
    }

    public void Distribute()
    {
        engine.Distribute(200, 200);
    }
}

public class PersonEdge : IEdge<PersonNode>
{
    public PersonEdge(PersonNode source, PersonNode target, double weight)
    {
        Source = source;
        Target = target;
        Weight = weight;
    }

    public PersonNode Source { get; }
    public PersonNode Target { get; }
    public double Weight { get; }
}

public class PersonNode : INode2D
{
    public Person Person { get; }

    public PersonNode(Person person)
    {
        Person = person;
    }

    public double ForceX { get; set; }
    public double ForceY { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class Graph2D<T, Q>(List<T> nodes, List<Q> edges) where T : INode2D where Q : IEdge<T>
{
    public List<T> Nodes { get; } = nodes;
    public List<Q> Edges { get; } = edges;
}