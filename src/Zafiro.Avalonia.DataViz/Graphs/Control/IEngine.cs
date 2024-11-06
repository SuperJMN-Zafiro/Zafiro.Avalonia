namespace Zafiro.Avalonia.DataViz.Graph.Control;

public interface IEngine
{
    void Step();
    void Distribute(double width, double height);
}