using System.Collections;

namespace Zafiro.Avalonia.Graphs.Control;

public interface IGraph
{
    IEnumerable Nodes { get; }
    IEnumerable Edges { get; }
}