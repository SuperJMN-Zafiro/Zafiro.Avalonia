using Zafiro.Avalonia.Controls.Diagrams;

namespace TestApp.Samples.Diagrams.Simple;

public class Edge : IHaveFromTo
{
    public Node From { get; }
    public Node To { get; }

    object IHaveFromTo.From => From;
    object IHaveFromTo.To => To;

    public Edge(Node from, Node to)
    {
        From = from;
        To = to;
    }
}