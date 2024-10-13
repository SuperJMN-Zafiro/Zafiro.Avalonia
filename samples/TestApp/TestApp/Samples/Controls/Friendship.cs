using Zafiro.Avalonia.DataViz.Graph.Core;

namespace TestApp.Samples.Controls;

public class Friendship : IEdge<Person>, IEdge2D
{
    public Friendship(Person source, Person target, double weight)
    {
        Source = source;
        Target = target;
        Weight = weight;
    }

    public Person Source { get; }
    public Person Target { get; }
    public double Weight { get; }

    INode2D IEdge<INode2D>.Target => Target;
    INode2D IEdge<INode2D>.Source => Source;
}