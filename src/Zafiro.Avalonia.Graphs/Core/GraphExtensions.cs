using System.Collections.Generic;
using System.Linq;
using Memoizer;

namespace Zafiro.Avalonia.Graphs.Core;

public static class GraphExtensions
{
    [Cache]
    public static double RelativeDegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        var nodeImportance = DegreeCentrality(graph, node);
        var maxImportance = MaxDegreeCentrality(graph);

        return maxImportance > 0 ? nodeImportance / maxImportance : 0;
    }

    [Cache]
    public static double MaxDegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph)
        where TEdge : IEdge<TNode>
    {
        if (!graph.Nodes.Any()) return 0;

        return graph.Nodes.Max(node => DegreeCentrality(graph, node));
    }

    [Cache]
    public static double DegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        var adjacent = graph.AdjacentEdges(node);
        var degreeCentrality = adjacent.Sum(edge => edge.Weight);
        return degreeCentrality;
    }

    [Cache]
    public static IEnumerable<TEdge> AdjacentEdges<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        var adjacentEdges = graph.Edges.Where(edge => Equals(edge.Source, node));
        var list = adjacentEdges.ToList();
        return list;
    }
}