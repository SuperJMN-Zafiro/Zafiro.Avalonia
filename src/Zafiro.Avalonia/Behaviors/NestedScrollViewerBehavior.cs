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

    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return Disposable.Empty;
        }

        return Observable.FromEventPattern<EventHandler, EventArgs>(
                h => AssociatedObject.LayoutUpdated += h,
                h =>
                {
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.LayoutUpdated -= h;
                    }
                })
            .Select(_ => GetInnerScrollBars())
            .DistinctUntilChanged()
            .Where(_ => IsEnabled)
            .Do(UpdateScrollBars)
            .Subscribe();
    }

    private (bool HasVertical, bool HasHorizontal) GetInnerScrollBars()
    {
        var scrollBars = AssociatedObject!
            .GetVisualDescendants()
            .OfType<ScrollBar>()
            .Where(sb => sb.GetSelfAndVisualAncestors().OfType<ScrollViewer>().FirstOrDefault() != AssociatedObject)
            .ToList();

        var hasVertical = scrollBars.Any(sb => sb.Orientation == Orientation.Vertical);
        var hasHorizontal = scrollBars.Any(sb => sb.Orientation == Orientation.Horizontal);

        return (hasVertical, hasHorizontal);
    }

    private void UpdateScrollBars((bool HasVertical, bool HasHorizontal) state)
    {
        var (hasVertical, hasHorizontal) = state;

        if (DisableVerticalScroll)
        {
            AssociatedObject!.VerticalScrollBarVisibility = hasVertical ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        }

        if (DisableHorizontalScroll)
        {
            AssociatedObject!.HorizontalScrollBarVisibility = hasHorizontal ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        }
    }
}