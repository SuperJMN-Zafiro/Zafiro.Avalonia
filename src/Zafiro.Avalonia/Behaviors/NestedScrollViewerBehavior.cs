using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

/// <summary>
/// Gets or sets whether vertical scrolling should be disabled when a nested ScrollViewer is detected.
/// </summary>
public class NestedScrollViewerBehavior : AttachedToVisualTreeBehavior<ScrollViewer>
{
    public static readonly StyledProperty<bool> DisableVerticalScrollProperty = AvaloniaProperty.Register<NestedScrollViewerBehavior, bool>(
        nameof(DisableVerticalScroll), true);


    public static readonly StyledProperty<bool> DisableHorizontalScrollProperty = AvaloniaProperty.Register<NestedScrollViewerBehavior, bool>(
        nameof(DisableHorizontalScroll));

    public bool DisableVerticalScroll
    {
        get => GetValue(DisableVerticalScrollProperty);
        set => SetValue(DisableVerticalScrollProperty, value);
    }

    public bool DisableHorizontalScroll
    {
        get => GetValue(DisableHorizontalScrollProperty);
        set => SetValue(DisableHorizontalScrollProperty, value);
    }

    private string? associatedObjectName;

    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        associatedObjectName = AssociatedObject.Name ?? AssociatedObject.GetType().Name ?? "UnknownScrollViewer";
        
        HandleNestedScrollBar()
            .DisposeWith(disposable);
    }

    private IDisposable HandleNestedScrollBar()
    {
        return Observable.FromEventPattern(h => AssociatedObject.LayoutUpdated += h, h =>
            {
                if (AssociatedObject != null)
                {
                    AssociatedObject.LayoutUpdated -= h;
                }
            })
            .Select(_ => AssociatedObject.FindDescendantOfType<ScrollBar>())
            .DistinctUntilChanged()
            .Where(_ => IsEnabled)
            .Subscribe(scrollBar =>
            {
                if (scrollBar is null)
                {
                    return;
                }

                // if (scrollBar.Orientation == Orientation.Horizontal && scrollBar.Visibility != ScrollBarVisibility.Disabled && DisableHorizontalScroll)
                // {
                //     AssociatedObject.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                // }
                // else
                // {
                //     AssociatedObject.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                // }

                if (scrollBar.Orientation == Orientation.Vertical && scrollBar.Visibility != ScrollBarVisibility.Disabled && DisableVerticalScroll)
                {
                    AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
                else
                {
                    AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            });
    }
}