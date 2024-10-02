public class Graph<TNode, TEdge> : IGraph<TNode, TEdge> where TEdge : IEdge<TNode>
{
    public List<TNode> Nodes { get; }
    public List<TEdge> Edges { get; }

    public Graph(List<TNode> nodes, List<TEdge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }
}