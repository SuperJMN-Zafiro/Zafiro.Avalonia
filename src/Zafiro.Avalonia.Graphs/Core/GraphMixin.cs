using System.Collections.Generic;
using System.Linq;

namespace Graphs;

public static class GraphMixin
{
    private static Dictionary<IGraph, double> maxImportanceDictionary = new();
    private static Dictionary<INode, double> importanceDictionary = new();
    private static Dictionary<INode, IEnumerable<IEdge>> edgesDictionary = new();

    public static double Importance(this INode node)
    {
        return importanceDictionary.GetOrAdd(node, () => node.Edges().Sum(x => x.Weight));
    }

    public static double MaxImportance(this IGraph graph)
    {
        return maxImportanceDictionary.GetOrAdd(graph, () => graph.Nodes.Max(x => x.Importance()));
    }

    public static double RelativeImportance(this INode x)
    {
        return x.Importance() / x.Graph.MaxImportance();
    }
    
    public static IEnumerable<IEdge> Edges(this INode node)
    {
        return edgesDictionary.GetOrAdd(node, () => node.Graph.Edges.Where(edge => node.Graph.GetNode(edge.Source).Value == node.Value));
    }
}