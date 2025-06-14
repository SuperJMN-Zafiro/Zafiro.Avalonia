using System;

namespace Zafiro.Avalonia.DataViz.Graphs.Control;

public class LoadOptions
{
    public int EdgeBufferCount { get; init; } = 20;
    public TimeSpan AddDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    public int VertexBufferCount { get; init; } = 10;
}