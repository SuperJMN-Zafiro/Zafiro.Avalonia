using System.Collections;
using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Behaviors;

public class ChildrenVisibilityExposerBehavior : DisposingBehavior<ItemsControl>
{
    public static readonly DirectProperty<ChildrenVisibilityExposerBehavior, IEnumerable?> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<ChildrenVisibilityExposerBehavior, IEnumerable?>(
        nameof(VisibleItems), o => o.VisibleItems, (o, v) => o.VisibleItems = v);

    public static readonly DirectProperty<ChildrenVisibilityExposerBehavior, IEnumerable?> InvisibleItemsProperty = AvaloniaProperty.RegisterDirect<ChildrenVisibilityExposerBehavior, IEnumerable?>(
        nameof(InvisibleItems), o => o.InvisibleItems, (o, v) => o.InvisibleItems = v);

    private IEnumerable? invisibleItems;

    private IEnumerable? visibleItems;

    public IEnumerable? VisibleItems
    {
        get => visibleItems;
        set => SetAndRaise(VisibleItemsProperty, ref visibleItems, value);
    }

    public IEnumerable? InvisibleItems
    {
        get => invisibleItems;
        set => SetAndRaise(InvisibleItemsProperty, ref invisibleItems, value);
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
            .Select(_ => AssociatedObject.ItemsPanelRoot)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(panel => new VisibleChildrenWatcher(panel))
            .Do(watcher => serialDisposable.Disposable = watcher)
            .Select(watcher => watcher.InvisibleChildren.ToObservableChangeSet())
            .Switch()
            .Transform(visual => AssociatedObject.ItemFromContainer((Control)visual)!)
            .Bind(out var invisibleItemsCollection)
            .Subscribe()
            .DisposeWith(disposables);

        Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromTicks(100), AvaloniaScheduler.Instance)
            .Select(_ => AssociatedObject.ItemsPanelRoot)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(panel => new VisibleChildrenWatcher(panel))
            .Do(watcher => serialDisposable.Disposable = watcher)
            .Select(watcher => watcher.VisibleChildren.ToObservableChangeSet())
            .Switch()
            .Transform(visual => AssociatedObject.ItemFromContainer((Control)visual)!)
            .Bind(out var visibleItemsCollection)
            .Subscribe()
            .DisposeWith(disposables);

        VisibleItems = visibleItemsCollection;
        InvisibleItems = invisibleItemsCollection;

        return disposables;
    }
}