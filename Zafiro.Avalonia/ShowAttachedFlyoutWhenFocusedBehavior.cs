using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia;

public class ShowAttachedFlyoutWhenFocusedBehavior : Behavior<Control>
{
    private readonly CompositeDisposable disposables = new();

    public static readonly StyledProperty<bool> IsFlyoutOpenProperty = AvaloniaProperty.Register<ShowAttachedFlyoutWhenFocusedBehavior, bool>(
        nameof(IsFlyoutOpen));

    public bool IsFlyoutOpen
    {
        get => GetValue(IsFlyoutOpenProperty);
        set => SetValue(IsFlyoutOpenProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (AssociatedObject is null)
        {
            return;
        }

        var flyoutBase = FlyoutBase.GetAttachedFlyout(AssociatedObject);
        if (flyoutBase is null)
        {
            return;
        }

        if (AssociatedObject.GetVisualRoot() is not Control visualRoot)
        {
            return;
        }

        var flyoutController = new FlyoutController(flyoutBase, AssociatedObject)
            .DisposeWith(disposables);

        DescendantPressed(visualRoot)
            .Select(descendant => AssociatedObject.IsVisualAncestorOf(descendant))
            .Do(isAncestor =>
            {
                flyoutController.IsOpen = isAncestor;
                IsFlyoutOpen = isAncestor;
            })
            .Subscribe()
            .DisposeWith(disposables);

        Observable.FromEventPattern(AssociatedObject, nameof(InputElement.GotFocus))
            .Do(_ =>
            {
                flyoutController.IsOpen = true;
                IsFlyoutOpen = true;
            })
            .Subscribe()
            .DisposeWith(disposables);

        Observable.FromEventPattern(AssociatedObject, nameof(InputElement.LostFocus))
            .Where(_ => !IsFocusInside(flyoutBase))
            .Do(_ =>
            {
                flyoutController.IsOpen = false;
                IsFlyoutOpen = false;
            })
            .Subscribe()
            .DisposeWith(disposables);

        this.GetObservable(IsFlyoutOpenProperty).Subscribe(b => flyoutController.IsOpen = b);
    }

    protected override void OnDetachedFromVisualTree()
    {
        disposables.Dispose();
    }

    private static IObservable<Visual> DescendantPressed(Control visualRoot)
    {
        return visualRoot.OnEvent(InputElement.PointerPressedEvent, RoutingStrategies.Tunnel)
            .Select(ea => ea.EventArgs.Source as Visual)
            .Where(s => s is not null)
            .Where(s => s is not LightDismissOverlayLayer);
    }

    private static bool IsFocusInside(IPopupHostProvider popupHostProvider)
    {
        var focusManager = FocusManager.Instance;

        if (focusManager?.Current is null)
        {
            return false;
        }

        var popupPresenter = popupHostProvider.PopupHost?.Presenter;

        if (popupPresenter is null)
        {
            return false;
        }

        var currentlyFocused = focusManager.Current;
        return popupPresenter.IsVisualAncestorOf(currentlyFocused);
    }
}