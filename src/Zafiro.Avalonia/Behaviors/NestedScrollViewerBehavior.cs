using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public class NestedScrollViewerBehavior : AttachedToVisualTreeBehavior<ScrollViewer>
{
    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        Observable.FromEventPattern(h => AssociatedObject.LayoutUpdated += h, h => AssociatedObject.LayoutUpdated -= h)
            .Select(a => AssociatedObject.FindDescendantOfType<ScrollViewer>())
            .Select(viewer => viewer != null)
            .Do(canScroll =>
            {
                var scrollBarVisibility = canScroll ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
                AssociatedObject.VerticalScrollBarVisibility = scrollBarVisibility;
                AssociatedObject.HorizontalScrollBarVisibility = scrollBarVisibility;
            })
            .Subscribe()
            .DisposeWith(disposable);
    }
}