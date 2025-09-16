using System.Collections;
using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.Monitoring;

namespace Zafiro.Avalonia.Behaviors;

public class ChildrenVisibilityExposerBehavior : DisposingBehavior<ItemsControl>
{
    public static readonly DirectProperty<ChildrenVisibilityExposerBehavior, IEnumerable?> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<ChildrenVisibilityExposerBehavior, IEnumerable?>(
        nameof(VisibleItems), o => o.VisibleItems, (o, v) => o.VisibleItems = v);

    private IEnumerable? visibleItems;

    public IEnumerable? VisibleItems
    {
        get => visibleItems;
        set => SetAndRaise(VisibleItemsProperty, ref visibleItems, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        var disposables = new CompositeDisposable();

        if (AssociatedObject == null)
        {
            throw new InvalidOperationException("AssociatedObject is null");
        }

        var serialDisposable = new SerialDisposable().DisposeWith(disposables);

        Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromTicks(100), AvaloniaScheduler.Instance)
            .Select(l => AssociatedObject.ItemsPanelRoot)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(panel => new ChildWatcher(panel))
            .Do(watcher => serialDisposable.Disposable = watcher)
            .Select(watcher => watcher.VisibleChildren.ToObservableChangeSet())
            .Switch()
            .Transform(visual => AssociatedObject.ItemFromContainer((Control)visual)!)
            .Bind(out var visibleItemsCollection)
            .Subscribe()
            .DisposeWith(disposables);

        VisibleItems = visibleItemsCollection;

        return disposables;
    }
}