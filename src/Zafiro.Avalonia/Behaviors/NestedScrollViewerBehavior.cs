using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
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

        Observable.FromEventPattern(h => AssociatedObject.LayoutUpdated += h, h => AssociatedObject.LayoutUpdated -= h)
            .Select(_ => AssociatedObject.FindDescendantOfType<ScrollViewer>())
            .DistinctUntilChanged()
            .Where(_ => IsEnabled)
            .Subscribe(nestedViewer =>
            {
                var hasNestedScrollViewer = nestedViewer != null;
                var scrollBarVisibility = hasNestedScrollViewer ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;

                if (DisableHorizontalScroll)
                {
                    AssociatedObject.HorizontalScrollBarVisibility = scrollBarVisibility;
                    LogScrollChange("Horizontal", hasNestedScrollViewer);
                }

                if (DisableVerticalScroll)
                {
                    AssociatedObject.VerticalScrollBarVisibility = scrollBarVisibility;
                    LogScrollChange("Vertical", hasNestedScrollViewer);
                }
            })
            .DisposeWith(disposable);
    }

    private void LogScrollChange(string direction, bool isDisabled)
    {
        var state = isDisabled ? "disabled" : "enabled";
        Debug.WriteLine($"[NestedScrollViewer] {direction} scroll for '{associatedObjectName}' has been {state} at {DateTime.Now:HH:mm:ss.fff}");
    }
}