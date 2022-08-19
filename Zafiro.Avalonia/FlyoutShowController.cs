using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia;

public class FlyoutShowController : IDisposable
{
    private readonly FlyoutBase flyout;
    private readonly Control parent;
    private bool isForcedOpen;

    public FlyoutShowController(Control parent, FlyoutBase flyout)
    {
        this.flyout = flyout;
        this.parent = parent;
    }

    public void SetIsForcedOpen(bool value)
    {
        if (isForcedOpen == value)
        {
            return;
        }

        isForcedOpen = value;

        if (isForcedOpen)
        {
            flyout.Closing += RejectClose;
            flyout.ShowAt(parent);
        }
        else
        {
            flyout.Closing -= RejectClose;
            flyout.Hide();
        }
    }

    private static void RejectClose(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public void Dispose()
    {
        flyout.Closing -= RejectClose;
    }
}