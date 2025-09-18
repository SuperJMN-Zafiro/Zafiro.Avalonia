using System.Reactive.Disposables;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls;

// Strict template-based presenter: swaps content when its own width crosses a breakpoint.
// Users cannot set Content; only Narrow and Wide are accepted.
public class ResponsivePresenter : TemplatedControl
{
    public static readonly StyledProperty<Control?> NarrowProperty =
        AvaloniaProperty.Register<ResponsivePresenter, Control?>(nameof(Narrow));

    public static readonly StyledProperty<Control?> WideProperty =
        AvaloniaProperty.Register<ResponsivePresenter, Control?>(nameof(Wide));

    public static readonly StyledProperty<double> BreakpointProperty =
        AvaloniaProperty.Register<ResponsivePresenter, double>(nameof(Breakpoint), 900);

    private readonly SerialDisposable subscriptions = new();
    private Control? current;
    private bool? isWide; // null = unknown, prevents redundant updates
    private ContentPresenter? presenter;

    public Control? Narrow
    {
        get => GetValue(NarrowProperty);
        set => SetValue(NarrowProperty, value);
    }

    public Control? Wide
    {
        get => GetValue(WideProperty);
        set => SetValue(WideProperty, value);
    }

    public double Breakpoint
    {
        get => GetValue(BreakpointProperty);
        set => SetValue(BreakpointProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        presenter = e.NameScope.Find<ContentPresenter>("PART_Presenter");
        UpdatePresenter();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var cd = new CompositeDisposable();

        var widthChanges = this.GetObservable(BoundsProperty)
            .Select(b => b.Width)
            .Where(w => w > 0);

        var breakpoints = this.GetObservable(BreakpointProperty)
            .StartWith(Breakpoint)
            .DistinctUntilChanged();

        widthChanges
            .CombineLatest(breakpoints, (w, bp) => w >= bp)
            .Sample(TimeSpan.FromMilliseconds(150), AvaloniaScheduler.Instance)
            .DistinctUntilChanged()
            .Do(wide =>
            {
                isWide = wide;
                var desired = wide ? Wide ?? Narrow : Narrow ?? Wide;
                if (!ReferenceEquals(current, desired))
                {
                    current = desired;
                    UpdatePresenter();
                }
            })
            .Subscribe()
            .DisposeWith(cd);

        subscriptions.Disposable = cd;

        UpdateContentIfNeeded();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        subscriptions.Disposable = Disposable.Empty;

        isWide = null;

        if (presenter != null)
        {
            presenter.Content = null;
        }

        current = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // If templates or breakpoint change, reevaluate immediately.
        if (change.Property == BreakpointProperty ||
            change.Property == NarrowProperty ||
            change.Property == WideProperty)
        {
            UpdateContentIfNeeded(force: true);
        }
    }

    private void UpdateContentIfNeeded(bool force = false)
    {
        var width = Bounds.Width;
        if (width <= 0)
            return; // Not arranged yet; skip spurious passes.

        var nowWide = width >= Breakpoint;

        if (!force && isWide == nowWide)
            return;

        isWide = nowWide;

        var desired = nowWide ? Wide ?? Narrow : Narrow ?? Wide;
        if (ReferenceEquals(current, desired))
            return;

        current = desired;
        UpdatePresenter();
    }

    private void UpdatePresenter()
    {
        if (presenter is null)
            return;

        presenter.Content = current;
    }
}