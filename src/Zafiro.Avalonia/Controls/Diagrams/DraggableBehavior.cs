using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Data;
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

    public static readonly StyledProperty<double> LeftProperty =
        AvaloniaProperty.Register<DraggableBehavior, double>(nameof(Left), defaultBindingMode : BindingMode.TwoWay);

    public static readonly StyledProperty<double> TopProperty =
        AvaloniaProperty.Register<DraggableBehavior, double>(nameof(Top), defaultBindingMode: BindingMode.TwoWay);

    public RoutingStrategies RoutingStrategy
    {
        get => GetValue(RoutingStrategyProperty);
        set => SetValue(RoutingStrategyProperty, value);
    }

    public double Left
    {
        get => GetValue(LeftProperty);
        set => SetValue(LeftProperty, value);
    }

    public double Top
    {
        get => GetValue(TopProperty);
        set => SetValue(TopProperty, value);
    }

    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject == null) return;

        PointerDownPoints(AssociatedObject)
            .Select(point => DeltasFor(AssociatedObject, point))
            .Switch()
            .TakeUntil(PointerReleased(AssociatedObject))
            .Repeat()
            .Do(diff => ApplyDelta(diff))
            .Subscribe()
            .DisposeWith(disposable);
    }

    private void ApplyDelta(Point diff)
    {
        if (!IsEnabled) return;

        var current = new Point(Left, Top);
        var next = current - diff;
        Left = next.X;
        Top= next.Y;
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