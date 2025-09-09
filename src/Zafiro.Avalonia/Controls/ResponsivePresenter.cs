namespace Zafiro.Avalonia.Controls;

using System;

// Strict template-based presenter: swaps content when its own width crosses a breakpoint.
// Users cannot set Content; only Narrow and Wide are accepted.
public class ResponsivePresenter : global::Avalonia.Controls.Primitives.TemplatedControl
{
public static readonly StyledProperty<global::Avalonia.Controls.Control?> NarrowProperty =
        AvaloniaProperty.Register<ResponsivePresenter, global::Avalonia.Controls.Control?>(nameof(Narrow));

public static readonly StyledProperty<global::Avalonia.Controls.Control?> WideProperty =
        AvaloniaProperty.Register<ResponsivePresenter, global::Avalonia.Controls.Control?>(nameof(Wide));

    public static readonly StyledProperty<double> BreakpointProperty =
        AvaloniaProperty.Register<ResponsivePresenter, double>(nameof(Breakpoint), 900);

    private IDisposable? _boundsSub;
    private bool? _isWide; // null = unknown, prevents redundant updates
private global::Avalonia.Controls.Presenters.ContentPresenter? _presenter;
    private global::Avalonia.Controls.Control? _current;

public global::Avalonia.Controls.Control? Narrow
    {
        get => GetValue(NarrowProperty);
        set => SetValue(NarrowProperty, value);
    }

public global::Avalonia.Controls.Control? Wide
    {
        get => GetValue(WideProperty);
        set => SetValue(WideProperty, value);
    }

    public double Breakpoint
    {
        get => GetValue(BreakpointProperty);
        set => SetValue(BreakpointProperty, value);
    }

protected override void OnApplyTemplate(global::Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
_presenter = e.NameScope.Find<global::Avalonia.Controls.Presenters.ContentPresenter>("PART_Presenter");
        UpdatePresenter();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _boundsSub = this.GetObservable(BoundsProperty).Subscribe(_ => UpdateContentIfNeeded());
        UpdateContentIfNeeded();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _boundsSub?.Dispose();
        _boundsSub = null;
        _isWide = null;
        if (_presenter != null)
            _presenter.Content = null;
        _current = null;
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

        if (!force && _isWide == nowWide)
            return;

        _isWide = nowWide;

var desired = nowWide ? Wide ?? Narrow : Narrow ?? Wide;
        if (ReferenceEquals(_current, desired))
            return;

        _current = desired;
        UpdatePresenter();
    }

    private void UpdatePresenter()
    {
        if (_presenter is null)
            return;

        _presenter.Content = _current;
    }
}
