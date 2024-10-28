using System.Collections;
using System.Collections.Generic;
using Zafiro.Graphs;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

public class Graph2D(IEnumerable<INode2D> nodes, IEnumerable<IEdge2D> edges) : Graph<INode2D, IEdge2D>(nodes, edges), IGraph2D, IGraph
{
    IEnumerable IGraph.Nodes => Nodes;
    IEnumerable IGraph.Edges => Edges;
}