using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

public static class GraphExtensions
{
    private static class ObjectCache
    {
        private static readonly ConditionalWeakTable<object, Dictionary<object, object>> Calculations = new();

        public static object GetOrCalculate(object owner, object key, Func<object> calculation)
        {
            var cache = Calculations.GetOrCreateValue(owner);
            if (cache.TryGetValue(key, out var cachedValue))
            {
                return cachedValue;
            }

            var result = calculation();
            cache[key] = result;

            return result;
        }
    }
    
    public static double RelativeDegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node) where TEdge : IEdge<TNode>
    {
        return (double) ObjectCache.GetOrCalculate(graph, (nameof(RelativeDegreeCentrality), node), () => RelativeDegreeCentralityCore(graph, node));
    }

    private static double RelativeDegreeCentralityCore<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        var nodeImportance = DegreeCentrality(graph, node);
        var maxImportance = MaxDegreeCentrality(graph);

        return maxImportance > 0 ? nodeImportance / maxImportance : 0;
    }


    public static double MaxDegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph)
        where TEdge : IEdge<TNode>
    {
        return (double) ObjectCache.GetOrCalculate(graph, nameof(MaxDegreeCentrality), () => MaxDegreeCentralityCore(graph));
    }

    private static double MaxDegreeCentralityCore<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph)
        where TEdge : IEdge<TNode>
    {
        if (!graph.Nodes.Any()) return 0;

        return graph.Nodes.Max(node => DegreeCentrality(graph, node));
    }

    public static double DegreeCentrality<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        return (double) ObjectCache.GetOrCalculate(graph, (nameof(DegreeCentrality), node), () => DegreeCentralityCore(graph, node));
    }

    private static double DegreeCentralityCore<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        var adjacent = graph.AdjacentEdges(node);
        var degreeCentrality = adjacent.Sum(edge => edge.Weight);
        return degreeCentrality;
    }

    public static IEnumerable<TEdge> AdjacentEdges<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        return (IEnumerable<TEdge>) ObjectCache.GetOrCalculate(graph, (nameof(AdjacentEdges), node), () => AdjacentEdgesCore(graph, node));
    }

    private static List<TEdge> AdjacentEdgesCore<TNode, TEdge>(this IGenericGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IEdge<TNode>
    {
        return graph.Edges.Where(edge => Equals(edge.Source, node)).ToList();
    }
}