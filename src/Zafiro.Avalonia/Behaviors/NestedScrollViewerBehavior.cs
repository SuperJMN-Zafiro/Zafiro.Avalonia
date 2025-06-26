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

    private string? associatedObjectName;

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

    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposable = new CompositeDisposable();

        if (AssociatedObject is null)
        {
            return disposable;
        }

        associatedObjectName = AssociatedObject.Name ?? AssociatedObject.GetType().Name ?? "UnknownScrollViewer";

        HandleNestedScrollBar()
            .DisposeWith(disposable);

        return disposable;
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
            .Select(_ => AssociatedObject!.GetVisualDescendants().OfType<ScrollBar>().ToList())
            .DistinctUntilChanged()
            .Where(_ => IsEnabled)
            .Subscribe(scrollBars =>
            {
                AssociatedObject!.VerticalScrollBarVisibility = GetVerticalVisibility(scrollBars);
                //AssociatedObject.HorizontalScrollBarVisibility = GetHorizontalVisibility(scrollBars);
            });
    }

    // TODO: Investigate issue here
    // private ScrollBarVisibility GetHorizontalVisibility(List<ScrollBar> scrollBars)
    // {
    //     var any = scrollBars.Any(scrollBar => scrollBar.Orientation == Orientation.Horizontal && scrollBar.Visibility != ScrollBarVisibility.Disabled && DisableHorizontalScroll);
    //     return any ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
    // }

    private ScrollBarVisibility GetVerticalVisibility(IEnumerable<ScrollBar> scrollBars)
    {
        var any = scrollBars.Any(scrollBar => scrollBar.Orientation == Orientation.Vertical && scrollBar.Visibility != ScrollBarVisibility.Disabled && DisableVerticalScroll);
        return any ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
    }
}