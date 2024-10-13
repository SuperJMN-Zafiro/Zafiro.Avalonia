using System.Collections;

namespace Zafiro.Avalonia.DataViz.Graph.Core;

public interface IGraph
{
    IEnumerable Nodes { get; }
    IEnumerable Edges { get; }
}