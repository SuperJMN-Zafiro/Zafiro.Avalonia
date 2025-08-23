using System.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Controls.Panels;

public enum OverflowDirection
{
    Horizontal,
    Vertical,
    Both
}

public class OverflowStateChangedEventArgs : RoutedEventArgs
{
    public OverflowStateChangedEventArgs(bool isOverflow)
    {
        IsOverflow = isOverflow;
    }

    public bool IsOverflow { get; init; }
}

public class AdaptivePanel : Panel
{
    private const double DefaultSwitchTolerance = 5.0;

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<AdaptivePanel, object?>(nameof(Content));

    public static readonly StyledProperty<object?> OverflowContentProperty =
        AvaloniaProperty.Register<AdaptivePanel, object?>(nameof(OverflowContent));

    public static readonly StyledProperty<bool> IsOverflowProperty =
        AvaloniaProperty.Register<AdaptivePanel, bool>(nameof(IsOverflow));

    public static readonly StyledProperty<OverflowDirection> OverflowDirectionProperty =
        AvaloniaProperty.Register<AdaptivePanel, OverflowDirection>(nameof(OverflowDirection), OverflowDirection.Both);

    public static readonly StyledProperty<double> SwitchToleranceProperty =
        AvaloniaProperty.Register<AdaptivePanel, double>(nameof(SwitchTolerance), DefaultSwitchTolerance);

    public static readonly StyledProperty<bool> EnableHysteresisProperty =
        AvaloniaProperty.Register<AdaptivePanel, bool>(nameof(EnableHysteresis), true);

    public static readonly RoutedEvent<OverflowStateChangedEventArgs> OverflowStateChangedEvent =
        RoutedEvent.Register<AdaptivePanel, OverflowStateChangedEventArgs>(nameof(OverflowStateChanged), RoutingStrategies.Bubble);

    private readonly ContentManager contentManager;
    private readonly LayoutCalculator layoutCalculator;
    private LayoutState currentState;

    public AdaptivePanel()
    {
        contentManager = new ContentManager(this);
        layoutCalculator = new LayoutCalculator();
        currentState = new LayoutState();
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object? OverflowContent
    {
        get => GetValue(OverflowContentProperty);
        set => SetValue(OverflowContentProperty, value);
    }

    public bool IsOverflow
    {
        get => GetValue(IsOverflowProperty);
        private set => SetValue(IsOverflowProperty, value);
    }

    public OverflowDirection OverflowDirection
    {
        get => GetValue(OverflowDirectionProperty);
        set => SetValue(OverflowDirectionProperty, value);
    }

    public double SwitchTolerance
    {
        get => GetValue(SwitchToleranceProperty);
        set => SetValue(SwitchToleranceProperty, Math.Max(0, value));
    }

    public bool EnableHysteresis
    {
        get => GetValue(EnableHysteresisProperty);
        set => SetValue(EnableHysteresisProperty, value);
    }

    public event EventHandler<OverflowStateChangedEventArgs>? OverflowStateChanged
    {
        add => AddHandler(OverflowStateChangedEvent, value);
        remove => RemoveHandler(OverflowStateChangedEvent, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        var result = HandlePropertyChange(change.Property);
        if (result.IsFailure)
        {
            // Log error if logging is available
            Debug.WriteLine($"AdaptivePanel property change failed: {result.Error}");
        }
    }

    private Result HandlePropertyChange(AvaloniaProperty property)
    {
        if (property == ContentProperty || property == OverflowContentProperty)
        {
            return contentManager.UpdateContent(Content, OverflowContent)
                .Tap(() => InvalidateMeasure());
        }

        if (property == OverflowDirectionProperty ||
            property == SwitchToleranceProperty ||
            property == EnableHysteresisProperty)
        {
            currentState = currentState.Reset();
            InvalidateMeasure();
            return Result.Success();
        }

        return Result.Success();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var measureResult = PerformMeasure(availableSize);

        return measureResult.Match(
            size => size,
            error =>
            {
                Debug.WriteLine($"AdaptivePanel measure failed: {error}");
                return new Size();
            });
    }

    private Result<Size> PerformMeasure(Size availableSize)
    {
        var contentResult = contentManager.GetContentSize();
        if (contentResult.IsFailure)
            return Result.Failure<Size>(contentResult.Error);

        var contentSize = contentResult.Value;

        var overflowDecision = layoutCalculator.ShouldUseOverflow(
            availableSize,
            contentSize,
            OverflowDirection,
            SwitchTolerance,
            EnableHysteresis,
            currentState);

        var newState = currentState.UpdateState(
            availableSize,
            contentSize,
            overflowDecision.IsOverflow);

        if (newState.HasStateChanged)
        {
            IsOverflow = newState.IsOverflow;
            RaiseOverflowStateChanged(newState);
        }

        currentState = newState;

        return contentManager.MeasureActiveContent(availableSize, IsOverflow);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangeResult = contentManager.ArrangeActiveContent(finalSize, IsOverflow);

        return arrangeResult.Match(
            size => size,
            error =>
            {
                Debug.WriteLine($"AdaptivePanel arrange failed: {error}");
                return finalSize;
            });
    }

    private void RaiseOverflowStateChanged(LayoutState state)
    {
        var args = new OverflowStateChangedEventArgs(state.IsOverflow)
        {
            RoutedEvent = OverflowStateChangedEvent
        };

        RaiseEvent(args);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        contentManager.Dispose();
        base.OnDetachedFromLogicalTree(e);
    }
}

// Helper classes for better separation of concerns

internal record LayoutState
{
    public bool IsOverflow { get; init; }
    public Size LastAvailableSize { get; init; }
    public Size LastContentSize { get; init; }
    public bool HasStateChanged { get; init; }

    public LayoutState UpdateState(Size availableSize, Size contentSize, bool isOverflow)
    {
        var hasChanged = IsOverflow != isOverflow ||
                         HasSizeChanged(LastAvailableSize, availableSize) ||
                         HasSizeChanged(LastContentSize, contentSize);

        return new LayoutState
        {
            IsOverflow = isOverflow,
            LastAvailableSize = availableSize,
            LastContentSize = contentSize,
            HasStateChanged = hasChanged
        };
    }

    private static bool HasSizeChanged(Size oldSize, Size newSize)
    {
        const double tolerance = 0.001;
        return Math.Abs(oldSize.Width - newSize.Width) > tolerance ||
               Math.Abs(oldSize.Height - newSize.Height) > tolerance;
    }

    public LayoutState Reset() => new();
}

internal record OverflowDecision(bool IsOverflow);

internal class LayoutCalculator
{
    public OverflowDecision ShouldUseOverflow(
        Size availableSize,
        Size contentSize,
        OverflowDirection direction,
        double tolerance,
        bool enableHysteresis,
        LayoutState currentState)
    {
        if (contentSize == default)
            return new OverflowDecision(false);

        var exceedsLimits = CheckExceedsLimits(availableSize, contentSize, direction);
        if (!exceedsLimits.HasValue)
            return new OverflowDecision(false);

        if (!exceedsLimits.Value)
            return new OverflowDecision(false);

        if (!enableHysteresis)
            return new OverflowDecision(true);

        return ApplyHysteresis(availableSize, contentSize, direction, tolerance, currentState.IsOverflow);
    }

    private bool? CheckExceedsLimits(Size availableSize, Size contentSize, OverflowDirection direction)
    {
        return direction switch
        {
            OverflowDirection.Horizontal when !double.IsInfinity(availableSize.Width) =>
                contentSize.Width > availableSize.Width,
            OverflowDirection.Vertical when !double.IsInfinity(availableSize.Height) =>
                contentSize.Height > availableSize.Height,
            OverflowDirection.Both =>
                (!double.IsInfinity(availableSize.Width) && contentSize.Width > availableSize.Width) ||
                (!double.IsInfinity(availableSize.Height) && contentSize.Height > availableSize.Height),
            _ => null
        };
    }

    private OverflowDecision ApplyHysteresis(
        Size availableSize,
        Size contentSize,
        OverflowDirection direction,
        double tolerance,
        bool currentlyOverflow)
    {
        var threshold = currentlyOverflow ? tolerance : -tolerance;

        var exceedsWithHysteresis = direction switch
        {
            OverflowDirection.Horizontal =>
                contentSize.Width > availableSize.Width + threshold,
            OverflowDirection.Vertical =>
                contentSize.Height > availableSize.Height + threshold,
            OverflowDirection.Both =>
                contentSize.Width > availableSize.Width + threshold ||
                contentSize.Height > availableSize.Height + threshold,
            _ => false
        };

        return new OverflowDecision(exceedsWithHysteresis);
    }
}

internal class ContentManager : IDisposable
{
    private readonly Panel panel;
    private Size? cachedContentSize;
    private Control? contentControl;
    private Control? overflowControl;

    public ContentManager(Panel panel)
    {
        this.panel = panel;
    }

    public void Dispose()
    {
        ClearContent();
        cachedContentSize = null;
    }

    public Result UpdateContent(object? content, object? overflowContent)
    {
        try
        {
            ClearContent();

            var contentResult = CreateControlFromContent(content);
            var overflowResult = CreateControlFromContent(overflowContent);

            if (contentResult.IsFailure)
                return Result.Failure($"Failed to create content control: {contentResult.Error}");

            if (overflowResult.IsFailure)
                return Result.Failure($"Failed to create overflow control: {overflowResult.Error}");

            contentControl = contentResult.Value;
            overflowControl = overflowResult.Value;

            AddControlsToPanel();
            cachedContentSize = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Unexpected error updating content: {ex.Message}");
        }
    }

    public Result<Size> GetContentSize()
    {
        if (contentControl == null)
            return Result.Success(new Size());

        if (cachedContentSize.HasValue)
            return Result.Success(cachedContentSize.Value);

        try
        {
            contentControl.Measure(Size.Infinity);
            cachedContentSize = contentControl.DesiredSize;
            return Result.Success(cachedContentSize.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<Size>($"Failed to measure content: {ex.Message}");
        }
    }

    public Result<Size> MeasureActiveContent(Size availableSize, bool useOverflow)
    {
        try
        {
            var activeControl = useOverflow ? overflowControl : contentControl;

            if (activeControl == null)
                return Result.Success(new Size());

            activeControl.Measure(availableSize);
            return Result.Success(activeControl.DesiredSize);
        }
        catch (Exception ex)
        {
            return Result.Failure<Size>($"Failed to measure active content: {ex.Message}");
        }
    }

    public Result<Size> ArrangeActiveContent(Size finalSize, bool useOverflow)
    {
        try
        {
            SetVisibility(useOverflow);

            var activeControl = useOverflow ? overflowControl : contentControl;
            activeControl?.Arrange(new Rect(finalSize));

            return Result.Success(finalSize);
        }
        catch (Exception ex)
        {
            return Result.Failure<Size>($"Failed to arrange content: {ex.Message}");
        }
    }

    private void SetVisibility(bool useOverflow)
    {
        if (contentControl != null)
            contentControl.IsVisible = !useOverflow;

        if (overflowControl != null)
            overflowControl.IsVisible = useOverflow;
    }

    private void ClearContent()
    {
        panel.Children.Clear();
        contentControl = null;
        overflowControl = null;
    }

    private void AddControlsToPanel()
    {
        if (contentControl != null)
            panel.Children.Add(contentControl);

        if (overflowControl != null)
            panel.Children.Add(overflowControl);
    }

    private static Result<Control?> CreateControlFromContent(object? content)
    {
        try
        {
            var control = content switch
            {
                null => null,
                Control c => c,
                string text => new TextBlock { Text = text },
                _ => new ContentPresenter { Content = content }
            };

            return Result.Success(control);
        }
        catch (Exception ex)
        {
            return Result.Failure<Control?>($"Failed to create control from content: {ex.Message}");
        }
    }
}