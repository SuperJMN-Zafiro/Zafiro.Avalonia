using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Behaviors;

public class ItemVisibilityMonitoringBehavior : DisposingBehavior<ItemsControl>
{
    public static readonly DirectProperty<ItemVisibilityMonitoringBehavior, ReadOnlyObservableCollection<object>?> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<ItemVisibilityMonitoringBehavior, ReadOnlyObservableCollection<object>?>(
        nameof(VisibleItems), o => o.VisibleItems, (o, v) => o.VisibleItems = v);

    public static readonly DirectProperty<ItemVisibilityMonitoringBehavior, ReadOnlyObservableCollection<object>?> InvisibleItemsProperty = AvaloniaProperty.RegisterDirect<ItemVisibilityMonitoringBehavior, ReadOnlyObservableCollection<object>?>(
        nameof(InvisibleItems), o => o.InvisibleItems, (o, v) => o.InvisibleItems = v);

    private ReadOnlyObservableCollection<object>? invisibleItems;

    private ReadOnlyObservableCollection<object>? visibleItems;

    public ReadOnlyObservableCollection<object>? VisibleItems
    {
        get => visibleItems;
        set => SetAndRaise(VisibleItemsProperty, ref visibleItems, value);
    }

    public ReadOnlyObservableCollection<object>? InvisibleItems
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

        var watcher = Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromTicks(100), AvaloniaScheduler.Instance)
            .Select(_ => AssociatedObject.ItemsPanelRoot)
            .WhereNotNull()
            .Take(1)
            .Select(panel => new VisibleChildrenWatcher(panel))
            .Do(watcher => serialDisposable.Disposable = watcher)
            .Publish()
            .RefCount();

        watcher
            .Select(watcher => watcher.InvisibleChildren.ToObservableChangeSet())
            .Switch()
            .Transform(visual => AssociatedObject.ItemFromContainer((Control)visual)!)
            .Bind(out var invisibleItemsCollection)
            .Subscribe()
            .DisposeWith(disposables);

        watcher
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