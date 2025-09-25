using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Misc;

public class VisibleChildrenWatcher : IDisposable
{
    private readonly CompositeDisposable disposable = new();

    public VisibleChildrenWatcher(Panel panel)
    {
        var childrenChanges = panel.Children.ToObservableChangeSetIfPossible()
            .ToSignal();

        var layoutChanges = Observable.FromEventPattern<EventHandler, EventArgs>(eh => panel.LayoutUpdated += eh, eh => panel.LayoutUpdated -= eh)
            .ToSignal()
            .Sample(TimeSpan.FromMilliseconds(16), AvaloniaScheduler.Instance);

        var visibleItemsChangeSet = layoutChanges.Merge(childrenChanges)
            .Select(_ => CalculateVisibleChildren(panel))
            .EditDiff(v => v, EqualityComparer<Visual>.Default);

        visibleItemsChangeSet
            .ToCollection()
            .Select(visible =>
            {
                var invisibles = panel.Children.Except(visible).ToList();
                return invisibles;
            })
            .EditDiff(visual => visual)
            .Bind(out var invisibleChildren)
            .Subscribe()
            .DisposeWith(disposable);

        visibleItemsChangeSet
            .Bind(out var visibleChildren)
            .Subscribe()
            .DisposeWith(disposable);

        VisibleChildren = visibleChildren;
        InvisibleChildren = invisibleChildren;
    }

    public ReadOnlyObservableCollection<Visual> InvisibleChildren { get; }

    public ReadOnlyObservableCollection<Visual> VisibleChildren { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private static IEnumerable<Visual> CalculateVisibleChildren(Panel panel)
    {
        // Intersect in panel's coordinate space: viewport is (0,0)-(panel.Width,panel.Height)
        var viewport = new Rect(panel.Bounds.Size);
        foreach (var panelChild in panel.Children)
        {
            if (viewport.Intersects(panelChild.Bounds))
            {
                yield return panelChild;
            }
        }
    }
}