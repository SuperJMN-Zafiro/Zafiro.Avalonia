using System.Collections;

namespace Zafiro.Avalonia.Graphs.Core;

public interface IGraph
{
    IEnumerable Nodes { get; }
    IEnumerable Edges { get; }
}