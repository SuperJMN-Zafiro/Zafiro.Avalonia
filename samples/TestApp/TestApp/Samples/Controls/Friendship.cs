using Zafiro.Avalonia.DataViz.Graph.Core;
using Zafiro.Avalonia.DataViz.Graphs.Core;
using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Controls;

public class Friendship : IWeightedEdge<Person>, IMutableEdge
{
    public Friendship(Person from, Person to, double weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }

    public Person From { get; }
    IMutableNode IEdge<IMutableNode>.To => To;

    IMutableNode IEdge<IMutableNode>.From => From;

    public Person To { get; }
    public double Weight { get; }
}