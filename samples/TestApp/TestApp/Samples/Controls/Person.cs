using Zafiro.Avalonia.Graphs.Core;

namespace TestApp.Samples.Controls;

public class Person(string name) : INode2D
{
    public string Name { get; } = name;
    public double ForceX { get; set; }
    public double ForceY { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}