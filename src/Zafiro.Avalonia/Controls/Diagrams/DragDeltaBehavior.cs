using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Zafiro.Avalonia.Mixins;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class DragDeltaBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategyProperty =
        AvaloniaProperty.Register<DragDeltaBehavior, RoutingStrategies>(nameof(RoutingStrategy),
            RoutingStrategies.Tunnel);

    public static readonly StyledProperty<double> LeftProperty =
        AvaloniaProperty.Register<DragDeltaBehavior, double>(nameof(Left), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> TopProperty =
        AvaloniaProperty.Register<DragDeltaBehavior, double>(nameof(Top), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<MouseButton> DragButtonProperty =
        AvaloniaProperty.Register<DragDeltaBehavior, MouseButton>(nameof(DragButton), MouseButton.Left);

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

    public MouseButton DragButton
    {
        get => GetValue(DragButtonProperty);
        set => SetValue(DragButtonProperty, value);
    }

    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject == null) return;

        var pointerPressed = PointerDownPoints(AssociatedObject);
        var pointerMoved = AssociatedObject.OnEvent(InputElement.PointerMovedEvent, RoutingStrategy)
            .Select(e => e.EventArgs.GetCurrentPoint(AssociatedObject).Position);
        var pointerReleased = PointerReleased(AssociatedObject);
        var captureLost = AssociatedObject.OnEvent(InputElement.PointerCaptureLostEvent);

        pointerPressed
            .Do(point => point.Pointer.Capture(AssociatedObject))
            .SelectMany(startPoint =>
                pointerMoved
                    .TakeUntil(pointerReleased
                        .Do(x => x.EventArgs.Pointer.Capture(null))
                        .ToSignal()
                        .Merge(captureLost.ToSignal()))
                    .Select(movePoint => startPoint.Position - movePoint)
                    .Do(ApplyDelta)
            )
            .Repeat()
            .Subscribe()
            .DisposeWith(disposable);
    }

    private void ApplyDelta(Point diff)
    {
        if (!IsEnabled)
        {
            return;
        }

        var current = new Point(Left, Top);
        var next = current - diff;
        Left = next.X;
        Top = next.Y;
    }

    private IObservable<EventPattern<PointerReleasedEventArgs>> PointerReleased(Control control)
    {
        return control.OnEvent(InputElement.PointerReleasedEvent, RoutingStrategy);
    }

    private IObservable<PointerPoint> PointerDownPoints(Control control)
    {
        return control.OnEvent(InputElement.PointerPressedEvent, RoutingStrategy)
            .Select(x => x.EventArgs.GetCurrentPoint(control))
            .Where(point => point.Properties.IsButtonPressed(DragButton));
    }
}