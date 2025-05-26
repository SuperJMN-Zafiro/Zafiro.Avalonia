using Zafiro.DataAnalysis.Graphs;

namespace TestApp.Samples.Misc;

public class Friendship : IWeightedEdge<Person>, IMutableEdge
{
    public Friendship(Person from, Person to, double weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }

    IMutableNode IEdge<IMutableNode>.To => To;

    IMutableNode IEdge<IMutableNode>.From => From;

    public Person From { get; }

    public Person To { get; }
    public double Weight { get; }
}