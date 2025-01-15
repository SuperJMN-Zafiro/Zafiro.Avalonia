namespace Zafiro.Avalonia.Controls.SuperCanvas;

public class NonOverlappingCanvas : SuperCanvas
{
    public NonOverlappingCanvas()
    {
        Layouters.Add(new NonOverlappingLayoutManager());
    }
}