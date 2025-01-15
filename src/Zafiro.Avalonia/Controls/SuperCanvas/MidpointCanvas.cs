namespace Zafiro.Avalonia.Controls.SuperCanvas;

public class MidpointCanvas : SuperCanvas
{
    public MidpointCanvas()
    {
        Layouters.Add(new MidpointLayoutManager());
    }
}