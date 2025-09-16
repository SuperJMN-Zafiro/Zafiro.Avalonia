using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Monitoring;

public class ChildWatcher : IDisposable
{
    private readonly CompositeDisposable disposable = new();

    public ChildWatcher(Panel panel)
    {
        var childrenChanges = panel.Children.ToObservableChangeSetIfPossible()
            .ToSignal();

        var layoutChanges = Observable.FromEventPattern<EventHandler, EventArgs>(eh => panel.LayoutUpdated += eh, eh => panel.LayoutUpdated -= eh)
            .ToSignal();

        var visibleItemsChangeSet = layoutChanges.Merge(childrenChanges)
            .Select(_ => CalculateVisibleChildren(panel))
            .EditDiff(v => v, EqualityComparer<Visual>.Default);

        visibleItemsChangeSet
            .ToCollection()
            .Select(visible => panel.Children.Except(visible))
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
        foreach (var panelChild in panel.Children)
        {
            if (panel.Bounds.Contains(panelChild.Bounds))
            {
                yield return panelChild;
            }
        }
    }
}