using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using DynamicData;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Monitoring;

public class ChildWatcherBehavior : DisposingBehavior<Panel>
{
    public static readonly DirectProperty<ChildWatcherBehavior, IEnumerable> VisibleChildrenProperty = AvaloniaProperty.RegisterDirect<ChildWatcherBehavior, IEnumerable>(
        nameof(VisibleChildren), o => o.VisibleChildren, (o, v) => o.VisibleChildren = v);

    private IEnumerable visibleChildren;

    public IEnumerable VisibleChildren
    {
        get => visibleChildren;
        set => SetAndRaise(VisibleChildrenProperty, ref visibleChildren, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        var disposable = new CompositeDisposable();

        if (AssociatedObject == null)
        {
            throw new InvalidOperationException("ChildWatcherBehavior must be attached to a Panel.");
        }

        var watcher = new ChildWatcher(AssociatedObject).DisposeWith(disposable);
        VisibleChildren = watcher.VisibleChildren;
        return watcher;
    }
}

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