using ReactiveUI;
using Zafiro.Avalonia.Graphs.Core;

namespace TestApp.Samples.Controls;

public class Person(string name, double weight) : ReactiveObject, INode2D
{
    private double x;
    private double y;
    public string Name { get; } = name;
    public double ForceX { get; set; }
    public double ForceY { get; set; }

    public double X
    {
        get => x;
        set => this.RaiseAndSetIfChanged(ref x, value);
    }

    public double Y
    {
        get => y;
        set => this.RaiseAndSetIfChanged(ref y, value);
    }

    public double Weight { get; } = weight;
}