using System.Collections.Generic;
using Zafiro.Avalonia.Graphs.Control;

namespace Zafiro.Avalonia.Graphs.Core;

public class Graph2D(List<INode2D> nodes, List<IEdge2D> edges) : IGraph2D
{
    public List<INode2D> Nodes { get; } = nodes;
    public List<IEdge2D> Edges { get; } = edges;
}

public interface IGraph2D
{
    public List<INode2D> Nodes { get; } 
    public List<IEdge2D> Edges { get; } 
}