using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Zafiro.Avalonia;

public class ShowAttachedFlyoutWhenFocusedBehavior : Behavior<Control>
{
	public static readonly DirectProperty<ShowAttachedFlyoutWhenFocusedBehavior, bool> IsFlyoutOpenProperty =
		AvaloniaProperty.RegisterDirect<ShowAttachedFlyoutWhenFocusedBehavior, bool>(
			"IsFlyoutOpen",
			o => o.IsFlyoutOpen,
			(o, v) => o.IsFlyoutOpen = v);

	private bool isFlyoutOpen;
    private CompositeDisposable disposable = new (Disposable.Empty);

    public bool IsFlyoutOpen
	{
		get => isFlyoutOpen;
		set => SetAndRaise(IsFlyoutOpenProperty, ref isFlyoutOpen, value);
	}

	protected override void OnAttachedToVisualTree()
	{
		if (AssociatedObject?.GetVisualRoot() is not Control visualRoot)
		{
			return;
		}

		var flyoutBase = FlyoutBase.GetAttachedFlyout(AssociatedObject);
		if (flyoutBase is null)
		{
			return;
		}

        disposable = new();
		var controller = new FlyoutShowController(AssociatedObject, flyoutBase).DisposeWith(disposable);

		FocusBasedFlyoutOpener(AssociatedObject, flyoutBase).DisposeWith(disposable);
		IsOpenPropertySynchronizer(controller).DisposeWith(disposable);

		// EDGE CASES
		// Edge case when the Visual Root becomes active and the Associated object is focused.
		ActivateOpener(AssociatedObject, visualRoot, controller).DisposeWith(disposable);
		DeactivateCloser(visualRoot, controller).DisposeWith(disposable);

		// This is a workaround for the case when the user switches theme. The same behavior is detached and re-attached on theme changes.
		// If you don't close it, the Flyout will show in an incorrect position. Maybe bug in Avalonia?
		if (IsFlyoutOpen)
		{
			controller.SetIsForcedOpen(false);
		}
	}

    protected override void OnDetachedFromVisualTree()
    {
        disposable.Dispose();
        base.OnDetachedFromVisualTree();
    }

    private static IDisposable DeactivateCloser(Control visualRoot, FlyoutShowController controller)
	{
		return Observable.FromEventPattern(visualRoot, nameof(Window.Deactivated))
			.Do(_ => controller.SetIsForcedOpen(false))
			.Subscribe();
	}

	private static IDisposable ActivateOpener(IInputElement associatedObject, Control visualRoot, FlyoutShowController controller)
	{
		return Observable.FromEventPattern(visualRoot, nameof(Window.Activated))
			.Where(_ => associatedObject.IsFocused)
			.Do(_ => controller.SetIsForcedOpen(true))
			.Subscribe();
	}

	private IDisposable IsOpenPropertySynchronizer(FlyoutShowController controller)
	{
		return this.WhenAnyValue(x => x.IsFlyoutOpen)
			.Do(controller.SetIsForcedOpen)
			.Subscribe();
	}

	private IDisposable FocusBasedFlyoutOpener(
		IAvaloniaObject associatedObject,
		FlyoutBase flyoutBase)
	{
		var isPopupFocused = GetPopupIsFocused(flyoutBase);
		var isAssociatedObjectFocused = associatedObject.GetObservable(InputElement.IsFocusedProperty);

        var mergedFocused = isAssociatedObjectFocused.Merge(isPopupFocused);

        var weAreFocused = mergedFocused
            .Buffer(TimeSpan.FromSeconds(0.1))
            .Where(focusedList => focusedList.Any())
            .Select(focusedList => focusedList.Last())
            .DistinctUntilChanged();
        
		return weAreFocused
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(isOpen => IsFlyoutOpen = isOpen)
            .Subscribe();
	}

    private static IObservable<bool> GetPopupIsFocused(FlyoutBase flyoutBase)
    {
        var currentPopupHost = Observable
            .FromEventPattern(flyoutBase, nameof(flyoutBase.Opened))
            .Select(_ => ((IPopupHostProvider) flyoutBase).PopupHost?.Presenter)
            .WhereNotNull();

        var popupGotFocus = currentPopupHost.Select(x => x.OnEvent(InputElement.GotFocusEvent)).Switch().ToSignal();
        var popupLostFocus = currentPopupHost.Select(x => x.OnEvent(InputElement.LostFocusEvent)).Switch().ToSignal();
        var flyoutGotFocus = popupGotFocus.Select(_ => true).Merge(popupLostFocus.Select(_ => false));
        return flyoutGotFocus;
    }
}