namespace Zafiro.Avalonia.Graphs.Tests;

public class Friendship : IEdge<Person>
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
}