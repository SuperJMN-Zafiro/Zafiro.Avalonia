public interface IGraph<TNode, TEdge> where TEdge : IEdge<TNode>
{
    List<TNode> Nodes { get; }
    List<TEdge> Edges { get; }
}