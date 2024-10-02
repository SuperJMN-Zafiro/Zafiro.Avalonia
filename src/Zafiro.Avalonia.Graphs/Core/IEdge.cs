namespace Graphs;

public interface IEdge<out T> : IEdge
{
    T Source { get; }
    T Target { get; }
    double Weight { get; }
}

public interface IEdge2D
{
    public INode2D Source { get; }
    public INode2D Target { get; }
    double Weight { get; }
}