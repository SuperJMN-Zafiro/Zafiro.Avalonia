using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class DraggableBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategyProperty =
        AvaloniaProperty.Register<DraggableBehavior, RoutingStrategies>(nameof(RoutingStrategy),
            RoutingStrategies.Tunnel);

    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<DraggableBehavior, bool>(nameof(IsEnabled), defaultValue: true);

    public RoutingStrategies RoutingStrategy
    {
        get => GetValue(RoutingStrategyProperty);
        set => SetValue(RoutingStrategyProperty, value);
    }

    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject == null) return;

        PointerDownPoints(AssociatedObject)
            .Select(point => DeltasFor(AssociatedObject, point))
            .Switch()
            .TakeUntil(PointerReleased(AssociatedObject))
            .Repeat()
            .Do(diff => ApplyDelta(AssociatedObject, diff))
            .Subscribe()
            .DisposeWith(disposable);
    }

    private void ApplyDelta(Control control, Point diff)
    {
        if (!IsEnabled) return;

        var current = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
        var next = current - diff;
        Canvas.SetLeft(control, next.X);
        Canvas.SetTop(control, next.Y);
    }

    private IObservable<EventPattern<PointerReleasedEventArgs>> PointerReleased(Control control)
    {
        return control.OnEvent(InputElement.PointerReleasedEvent, RoutingStrategy);
    }

    private IObservable<Point> PointerDownPoints(Control control)
    {
        return control.OnEvent(InputElement.PointerPressedEvent, RoutingStrategy)
            .Select(x => x.EventArgs.GetCurrentPoint(control))
            .Where(x => x.Pointer.IsPrimary)
            .Select(point => point.Position);
    }

    private IObservable<Point> DeltasFor(Control control, Point point)
    {
        return control.OnEvent(InputElement.PointerMovedEvent, RoutingStrategy)
            .Select(x => point - x.EventArgs.GetCurrentPoint(control).Position);
    }
}