namespace Zafiro.Avalonia.DataViz.Graph.Core;

public class Configuration
{
    public double RepulsionForce { get; set; } = 1000000;
    public double AttractionForce { get; set; } = 0.1;
    public double EquilibriumDistance { get; set; } = 200;
    public double Damping { get; set; } = 0.05;
    public double Epsilon { get; set; } = 0.01;
    public double Theta { get; set; } = 0.3; // Umbral para Barnes-Hut
    public int MaxNodesPerLeaf { get; set; } = 4;
}