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

        layoutChanges.Merge(childrenChanges)
            .Select(_ => CalculateVisibleChildren(panel))
            .EditDiff(v => v, EqualityComparer<Visual>.Default)
            .Bind(out var visibleChildren)
            .Subscribe()
            .DisposeWith(disposable);

        VisibleChildren = visibleChildren;
    }

    public ReadOnlyObservableCollection<Visual> VisibleChildren { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private IEnumerable<Visual> CalculateVisibleChildren(Panel panel)
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