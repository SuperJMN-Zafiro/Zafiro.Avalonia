public interface IEdge<T>
{
    T Source { get; }
    T Target { get; }
    double Weight { get; }
}