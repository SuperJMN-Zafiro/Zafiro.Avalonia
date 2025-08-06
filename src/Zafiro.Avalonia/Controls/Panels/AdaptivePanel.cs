using Avalonia.Controls.Presenters;
using Avalonia.Metadata;

namespace Zafiro.Avalonia.Controls.Panels;

public enum OverflowDirection
{
    Horizontal,
    Vertical,
    Both
}

public class AdaptivePanel : Panel
{
    private const double SwitchTolerance = 5.0;

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<AdaptivePanel, object?>(nameof(Content));

    public static readonly StyledProperty<object?> OverflowContentProperty =
        AvaloniaProperty.Register<AdaptivePanel, object?>(nameof(OverflowContent));

    public static readonly StyledProperty<bool> IsOverflowProperty =
        AvaloniaProperty.Register<AdaptivePanel, bool>(nameof(IsOverflow));

    public static readonly StyledProperty<OverflowDirection> OverflowDirectionProperty =
        AvaloniaProperty.Register<AdaptivePanel, OverflowDirection>(nameof(OverflowDirection));

    private Control? contentControl;
    private Size contentDesiredSize;
    private Size lastMeasuredSize;
    private bool lastOverflowState;
    private Control? overflowControl;

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty ||
            change.Property == OverflowContentProperty ||
            change.Property == OverflowDirectionProperty)
        {
            UpdateContent();
            InvalidateMeasure();
        }
    }

    private void UpdateContent()
    {
        Children.Clear();

        contentControl = CreateControlFromContent(Content);
        overflowControl = CreateControlFromContent(OverflowContent);

        if (contentControl != null)
        {
            Children.Add(contentControl);
        }

        if (overflowControl != null)
        {
            Children.Add(overflowControl);
        }

        // Reset cached sizes
        contentDesiredSize = new Size();
    }

    private Control? CreateControlFromContent(object? content)
    {
        return content switch
        {
            null => null,
            Control control => control,
            string text => new TextBlock { Text = text },
            _ => new ContentPresenter { Content = content }
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // Only measure content control once to get its natural size
        if (contentControl != null && contentDesiredSize.Width == 0 && contentDesiredSize.Height == 0)
        {
            // Temporarily make it visible to measure, but don't affect layout decision
            var wasVisible = contentControl.IsVisible;
            contentControl.IsVisible = true;
            contentControl.Measure(Size.Infinity);
            contentDesiredSize = contentControl.DesiredSize;
            contentControl.IsVisible = wasVisible;
        }

        // Determine which layout to use based on available space and hysteresis
        var shouldOverflow = ShouldUseOverflow(availableSize);

        // Only update if size changed significantly or state should change
        if (Math.Abs(lastMeasuredSize.Width - availableSize.Width) > SwitchTolerance ||
            shouldOverflow != lastOverflowState)
        {
            IsOverflow = shouldOverflow;
            lastOverflowState = shouldOverflow;
            lastMeasuredSize = availableSize;
        }

        // Measure and return size of active control
        if (IsOverflow && overflowControl != null)
        {
            overflowControl.IsVisible = true;
            overflowControl.Measure(availableSize);
            return overflowControl.DesiredSize;
        }

        if (contentControl != null)
        {
            // Use cached size to avoid re-measuring
            return contentDesiredSize;
        }

        return new Size();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Set visibility based on current state
        if (contentControl != null)
        {
            contentControl.IsVisible = !IsOverflow;
        }

        if (overflowControl != null)
        {
            overflowControl.IsVisible = IsOverflow;
        }

        // Arrange only the visible control
        if (IsOverflow && overflowControl != null && overflowControl.IsVisible)
        {
            overflowControl.Arrange(new Rect(finalSize));
        }
        else if (!IsOverflow && contentControl != null && contentControl.IsVisible)
        {
            contentControl.Arrange(new Rect(finalSize));
        }

        return finalSize;
    }

    private bool ShouldUseOverflow(Size availableSize)
    {
        // If no overflow content is defined, always use normal content
        if (overflowControl == null)
        {
            return false;
        }

        if (contentDesiredSize.Width == 0 && contentDesiredSize.Height == 0)
        {
            return false;
        }

        var exceedsLimits = OverflowDirection switch
        {
            OverflowDirection.Horizontal => !double.IsInfinity(availableSize.Width) &&
                                            contentDesiredSize.Width > availableSize.Width,
            OverflowDirection.Vertical => !double.IsInfinity(availableSize.Height) &&
                                          contentDesiredSize.Height > availableSize.Height,
            OverflowDirection.Both => (!double.IsInfinity(availableSize.Width) && contentDesiredSize.Width > availableSize.Width) ||
                                      (!double.IsInfinity(availableSize.Height) && contentDesiredSize.Height > availableSize.Height),
            _ => false
        };

        if (!exceedsLimits)
        {
            return false;
        }

        // Apply hysteresis based on direction
        if (lastOverflowState)
        {
            // Currently in overflow - switch back only if content fits comfortably
            return OverflowDirection switch
            {
                OverflowDirection.Horizontal => contentDesiredSize.Width > availableSize.Width + SwitchTolerance,
                OverflowDirection.Vertical => contentDesiredSize.Height > availableSize.Height + SwitchTolerance,
                OverflowDirection.Both => contentDesiredSize.Width > availableSize.Width + SwitchTolerance ||
                                          contentDesiredSize.Height > availableSize.Height + SwitchTolerance,
                _ => false
            };
        }

        // Currently showing content - switch to overflow if content doesn't fit
        return OverflowDirection switch
        {
            OverflowDirection.Horizontal => contentDesiredSize.Width > availableSize.Width - SwitchTolerance,
            OverflowDirection.Vertical => contentDesiredSize.Height > availableSize.Height - SwitchTolerance,
            OverflowDirection.Both => contentDesiredSize.Width > availableSize.Width - SwitchTolerance ||
                                      contentDesiredSize.Height > availableSize.Height - SwitchTolerance,
            _ => false
        };
    }
}