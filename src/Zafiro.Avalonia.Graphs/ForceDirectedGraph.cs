namespace Zafiro.Avalonia.Graphs;

public class ForceDirectedGraph<TNode2D, TEdge> where TNode2D : class, INode2D where TEdge : IEdge<TNode2D>
{
    public Graph2D<TNode2D, TEdge> Graph2d { get; }
    private readonly Engine<TNode2D, TEdge> engine;

    public ForceDirectedGraph(Graph2D<TNode2D, TEdge> graph2d)
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