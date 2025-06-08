using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors;

public class OverflowBehavior : Behavior<Control>, IDisposable
{
    public static readonly StyledProperty<int> DebounceMillisecondsProperty =
        AvaloniaProperty.Register<OverflowBehavior, int>(
            nameof(DebounceMilliseconds), 50);

    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private bool overflow;

    public int DebounceMilliseconds
    {
        get => GetValue(DebounceMillisecondsProperty);
        set => SetValue(DebounceMillisecondsProperty, value);
    }

    public void Dispose() => disposables.Dispose();

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;

        var layoutUpdated = Observable
            .FromEventPattern(
                h => AssociatedObject.LayoutUpdated += h,
                h => AssociatedObject.LayoutUpdated -= h)
            .ToSignal();

        var boundsChanged = AssociatedObject
            .GetObservable(Visual.BoundsProperty)
            .ToSignal();

        layoutUpdated
            .Merge(boundsChanged)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Throttle(TimeSpan.FromMilliseconds(DebounceMilliseconds), AvaloniaScheduler.Instance)
            .Subscribe(_ => UpdateState())
            .DisposeWith(disposables);
    }

    protected override void OnDetaching()
    {
        disposables.Dispose();
        base.OnDetaching();
    }

    private void UpdateState()
    {
        if (AssociatedObject == null)
            return;

        bool hasOverflow = CheckOverflow();
        if (hasOverflow == overflow)
            return; // sin cambio, no reaplicar

        overflow = hasOverflow;
        ApplyState();
    }

    private bool CheckOverflow()
    {
        // 1. Obtener el Panel que realmente contiene los ítems
        Panel panel = null;
        if (AssociatedObject is ItemsControl ic)
            panel = ic.ItemsPanelRoot;
        else if (AssociatedObject is Panel p)
            panel = p;

        if (panel == null)
            return false;

        // 2. Medir solo sus hijos directos
        var bounds = panel.Bounds;
        double total = 0;

        foreach (var child in panel.Children.OfType<Control>())
        {
            child.Measure(new Size(double.PositiveInfinity, bounds.Height));
            total += child.DesiredSize.Width;
            if (total > bounds.Width)
                return true;
        }

        return false;
    }

    private void ApplyState()
    {
        var classes = (IPseudoClasses)AssociatedObject.Classes;
        classes.Set(":overflow", overflow);
    }
}