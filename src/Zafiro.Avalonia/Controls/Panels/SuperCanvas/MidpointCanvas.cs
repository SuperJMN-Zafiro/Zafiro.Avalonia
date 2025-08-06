namespace Zafiro.Avalonia.Controls.Panels.SuperCanvas;

public class MidpointCanvas : SuperCanvas
{
    public MidpointCanvas()
    {
        Layouters.Add(new MidpointLayoutManager());
    }
}