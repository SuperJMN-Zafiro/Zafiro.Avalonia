using Zafiro.Avalonia.DataViz.Graph.Core;

namespace Zafiro.Avalonia.Graphs.Tests;

public class Person(string name) : INode2D
{
    public string Name { get; } = name;
    public double ForceX { get; set; }
    public double ForceY { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Weight { get; } = 0;
    public bool IsEnabled { get; }
}