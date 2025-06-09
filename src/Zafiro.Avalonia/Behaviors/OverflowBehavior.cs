using System.Reactive.Disposables;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Behaviors;

public class OverflowBehavior : Behavior<Control>, IDisposable
{
    public static readonly StyledProperty<int> DebounceMillisecondsProperty =
        AvaloniaProperty.Register<OverflowBehavior, int>(
            nameof(DebounceMilliseconds), 50);

    public static readonly StyledProperty<bool> ExcludeTextOverflowProperty = AvaloniaProperty.Register<OverflowBehavior, bool>(
        nameof(ExcludeTextOverflow), true);

    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private bool overflow;

    public int DebounceMilliseconds
    {
        get => GetValue(DebounceMillisecondsProperty);
        set => SetValue(DebounceMillisecondsProperty, value);
    }

    public bool ExcludeTextOverflow
    {
        get => GetValue(ExcludeTextOverflowProperty);
        set => SetValue(ExcludeTextOverflowProperty, value);
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
            .Select(_ => AssociatedObject.Bounds)
            .DistinctUntilChanged()
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
        Panel panel = AssociatedObject switch
        {
            ItemsControl ic => ic.ItemsPanelRoot,
            Panel p => p,
            _ => null
        };
        if (panel is null)
            return false;

        var bounds = panel.Bounds;
        double total = 0;

        foreach (Control child in panel.Children.OfType<Control>())
        {
            double remaining = bounds.Width - total;

            if (ExcludeTextOverflow && IsWrappableTextBranch(child))
            {
                var branch = new[] { child }
                    .Concat(child.GetVisualDescendants().OfType<Control>());

                double threshold = branch
                    .Select(c => c.MinWidth)
                    .DefaultIfEmpty(0)
                    .Max();

                if (remaining < threshold)
                    return true;

                // 2. Medir con el ancho restante, wrapping posible
                child.Measure(new Size(remaining, bounds.Height));
            }
            else
            {
                // Medición convencional (sin wrapping)
                child.Measure(new Size(double.PositiveInfinity, bounds.Height));
            }

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

    private bool IsWrappableTextBranch(Control control)
    {
        // Reunir el control raíz y sus descendientes que sean Controls
        var nodes = control
            .GetVisualDescendants()
            .OfType<Control>()
            .Prepend(control);

        // Filtrar sólo los TextBlock con wrapping activo
        var textBlocks = nodes
            .OfType<TextBlock>()
            .Where(tb => tb.TextWrapping != TextWrapping.NoWrap);

        if (!textBlocks.Any())
            return false;

        // Detectar otros controles “hoja” distintos de TextBlock
        var leaves = nodes
            .Where(c => !(c is Panel)
                        && !(c is Decorator)
                        && !(c is ContentPresenter)
                        && !(c is ItemsPresenter)
                        && !(c is TextBlock));

        return !leaves.Any();
    }
}