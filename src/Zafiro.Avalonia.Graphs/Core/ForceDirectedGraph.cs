using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs.Core;

public class ForceDirectedGraph : IGraph<INode2D, IEdge<INode2D>>
{
    public IGraph<INode2D, IEdge<INode2D>> Graph2d { get; }
    private readonly Engine engine;

    public ForceDirectedGraph(IGraph<INode2D, IEdge<INode2D>> graph2d)
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

    public List<INode2D> Nodes => Graph2d.Nodes;

    public List<IEdge<INode2D>> Edges => Graph2d.Edges;
}