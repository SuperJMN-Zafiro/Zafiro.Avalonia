namespace Zafiro.Avalonia.Controls;

using System;

// Simple, self-measuring presenter: swaps content when its *own* width crosses a breakpoint.
public class ResponsivePresenter : ContentControl
{
    public static readonly StyledProperty<Control?> NarrowProperty =
        AvaloniaProperty.Register<ResponsivePresenter, Control?>(nameof(Narrow));

    public static readonly StyledProperty<Control?> WideProperty =
        AvaloniaProperty.Register<ResponsivePresenter, Control?>(nameof(Wide));

    public static readonly StyledProperty<double> BreakpointProperty =
        AvaloniaProperty.Register<ResponsivePresenter, double>(nameof(Breakpoint), 900);

    private IDisposable? _boundsSub;
    private bool? _isWide; // null = unknown, prevents redundant Content sets

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
    }

    // Avalonia 11 overrides the non-generic OnPropertyChanged
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

        // Avoid reassigning Content unless the "mode" actually changes or caller forces it.
        if (!force && _isWide == nowWide)
            return;

        _isWide = nowWide;

        // Only one subtree is in the visual tree at a time.
        var desired = nowWide ? Wide ?? Narrow : Narrow ?? Wide;
        if (!ReferenceEquals(Content, desired))
            Content = desired;
    }
}
