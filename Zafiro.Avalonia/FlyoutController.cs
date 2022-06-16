using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia;

public class FlyoutController : IDisposable
{
    private readonly FlyoutBase flyout;
    private readonly Control parent;
    private bool isOpen;
    private readonly FlyoutCloseEnforcer enforcer;

    public FlyoutController(FlyoutBase flyout, Control parent)
    {
        this.flyout = flyout;
        this.parent = parent;
        enforcer = new FlyoutCloseEnforcer(flyout);
    }

    public bool IsOpen
    {
        get => isOpen;
        set
        {
            if (isOpen == value)
            {
                return;
            }

            ToggleFlyout(value);
            isOpen = value;
        }
    }

    private void ToggleFlyout(bool isVisible)
    {
        if (isVisible)
        {
            ShowFlyout();
        }
        else
        {
            HideFlyout();
        }
    }

    private void HideFlyout()
    {
        enforcer.IsForcedOpen = false;
        flyout.Hide();
    }

    private void ShowFlyout()
    {
        enforcer.IsForcedOpen = true;
        flyout.ShowAt(parent);
    }

    public void Dispose()
    {
        enforcer.Dispose();
    }
}