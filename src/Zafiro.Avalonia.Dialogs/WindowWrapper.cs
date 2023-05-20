using Avalonia.Controls;

namespace Zafiro.UI.Avalonia;

public class WindowWrapper : IWindow
{
    private readonly Window window;

    public WindowWrapper(Window window)
    {
        this.window = window;
    }

    public void Close()
    {
        window.Close();
    }
}