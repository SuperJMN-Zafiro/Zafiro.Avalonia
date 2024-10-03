using System.Collections.Generic;

namespace Zafiro.Avalonia.Graphs.Core;

public interface IGraph2D
{
    public List<INode2D> Nodes { get; } 
    public List<IEdge2D> Edges { get; } 
}