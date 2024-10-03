using Zafiro.Avalonia.Graphs.Core;

namespace TestApp.Samples.Controls;

public class Friendship : IEdge2D
{
    public Friendship(Person source, Person target, double weight)
    {
        Source = source;
        Target = target;
        Weight = weight;
    }

    public INode2D Source { get; }
    public INode2D Target { get; }
    public double Weight { get; }
}