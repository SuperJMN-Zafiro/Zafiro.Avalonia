namespace Zafiro.Avalonia.Controls.SuperCanvas;

public class MidpointLayoutManager : AvaloniaObject, ILayoutManager
{
    public void ProcessLayout(List<ControlPosition> positions)
    {
        foreach (var pos in positions)
        {
            pos.Left -= pos.Width / 2;
            pos.Top -= pos.Height / 2;
        }
    }
}