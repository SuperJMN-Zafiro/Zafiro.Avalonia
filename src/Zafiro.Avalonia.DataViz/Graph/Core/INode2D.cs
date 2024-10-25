namespace Zafiro.Avalonia.DataViz.Graph.Core;

public interface INode2D
{
    double ForceX { get; set; }
    double ForceY { get; set; }
    double X { get; set; }
    double Y { get; set; }
    public double Weight { get; }
    bool IsEnabled { get; }
}