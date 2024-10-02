namespace Graphs;

public class Configuration
{
    public double RepulsionForce { get; set; } = 10000;
    public double AttractionForce { get; set; } = 0.0000015;
    public double EquilibriumDistance { get; set; } = 200;
    public double Damping { get; set; } = 0.85;
}