using System.ComponentModel;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia;

public class FlyoutCloseEnforcer : IDisposable
{
    private readonly FlyoutBase flyout;
    private bool isForcedOpen;

    public FlyoutCloseEnforcer(FlyoutBase flyout)
    {
        this.flyout = flyout;
    }

    public bool IsForcedOpen
    {
        get => isForcedOpen;
        set
        {
            if (isForcedOpen == value)
            {
                return;
            }

            isForcedOpen = value;

            if (isForcedOpen)
            {
                flyout.Closing += RejectClose;
            }
            else
            {
                flyout.Closing -= RejectClose;
            }
        }
    }

    private static void RejectClose(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public void Dispose()
    {
        if (!IsForcedOpen)
        {
            flyout.Closing -= RejectClose;
        }
    }
}