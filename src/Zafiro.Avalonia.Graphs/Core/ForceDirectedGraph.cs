namespace Zafiro.Avalonia.Graphs.Core;

public class ForceDirectedGraph
{
    public Graph2D Graph2d { get; }
    private readonly Engine engine;

    public ForceDirectedGraph(Graph2D graph2d)
    {
        Graph2d = graph2d;
        engine = new Engine(graph2d);
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