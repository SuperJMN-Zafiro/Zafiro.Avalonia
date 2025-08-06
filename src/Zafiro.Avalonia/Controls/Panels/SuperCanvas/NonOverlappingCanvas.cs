namespace Zafiro.Avalonia.Controls.Panels.SuperCanvas;

public class NonOverlappingCanvas : SuperCanvas
{
    public NonOverlappingCanvas()
    {
        Layouters.Add(new NonOverlappingLayoutManager());
    }
}