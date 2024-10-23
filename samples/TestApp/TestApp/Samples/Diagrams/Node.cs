using Zafiro.Avalonia.Controls.Diagrams;

namespace TestApp.Samples.Diagrams;

public class Node(string name) : IHaveLocation
{
    public string Name { get; } = name;
    public double Left { get; set; }
    public double Top { get; set; }
}