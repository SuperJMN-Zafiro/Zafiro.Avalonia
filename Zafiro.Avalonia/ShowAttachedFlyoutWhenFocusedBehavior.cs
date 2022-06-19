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
	public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
		AvaloniaProperty.Register<ShowAttachedFlyoutWhenFocusedBehavior, bool>(
			nameof(IsFlyoutOpen));

	private readonly CompositeDisposable disposables = new();

	private FlyoutController flyoutController;

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

		flyoutController = new FlyoutController(flyoutBase, AssociatedObject)
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

	private static IObservable<Visual> DescendantPressed(IInteractive interactive)
	{
		return
			from eventPattern in interactive.OnEvent(InputElement.PointerPressedEvent, RoutingStrategies.Tunnel)
			let source = eventPattern.EventArgs.Source as Visual
			where source is not null
			where source is not LightDismissOverlayLayer
			select source;
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