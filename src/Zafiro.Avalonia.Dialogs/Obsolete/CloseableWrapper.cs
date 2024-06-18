using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.Obsolete;

public class CloseableWrapper : ICloseable
{
    private readonly Window window;

    public CloseableWrapper(Window window)
    {
        this.window = window;
    }

    public void Close()
    {
        window.Close(true);
    }

    public void Dismiss()
    {
        window.Close(false);
    }
}