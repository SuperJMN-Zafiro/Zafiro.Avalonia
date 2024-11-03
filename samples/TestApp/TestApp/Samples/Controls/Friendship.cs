using Zafiro.Avalonia.DataViz.Graph.Core;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Controls;

public class Friendship : IWeightedEdge<Person>, IEdge2D
{
    public Friendship(Person from, Person to, double weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }

    public Person From { get; }
    INode2D IEdge<INode2D>.To => To;

    INode2D IEdge<INode2D>.From => From;

    public Person To { get; }
    public double Weight { get; }
}