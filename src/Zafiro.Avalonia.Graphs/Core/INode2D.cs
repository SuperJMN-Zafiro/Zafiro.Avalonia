using Zafiro.Avalonia.Graphs.Core;

namespace Graphs;

public interface INode2D<T> : INode<T>
{
    IGraph2D<T> Graph2D { get; }
    public double X { get; set; }
    public double Y { get; set; }
    double ForceX { get; set; }
    double ForceY { get; set; }
}