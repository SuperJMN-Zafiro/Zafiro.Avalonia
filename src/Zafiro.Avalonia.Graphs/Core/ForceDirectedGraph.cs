using System.Collections.Generic;
using Zafiro.Avalonia.Graphs.Control;

namespace Zafiro.Avalonia.Graphs.Core;

public class ForceDirectedGraph : IGraph2D
{
    public IGraph2D Graph2d { get; }
    private readonly Engine engine;

    public ForceDirectedGraph(IGraph2D graph2d)
    {
        Graph2d = graph2d;
        engine = new Engine(graph2d);
    }

    public void Step()
    {
        engine.Step();
    }

    public void Distribute(double width, double height)
    {
        engine.Distribute(width, height);
    }

    public List<INode2D> Nodes => Graph2d.Nodes;

    public List<IEdge2D> Edges => Graph2d.Edges;
}