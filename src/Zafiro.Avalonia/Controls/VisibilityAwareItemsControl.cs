using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Interactivity;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.Monitoring;

namespace Zafiro.Avalonia.Controls;

/// <summary>
/// ItemsControl that exposes two reactive lists containing the data items whose containers are visible vs invisible
/// within the current viewport (ScrollViewer, ClipToBounds ancestor, or VisualRoot).
///
/// - Reactive approach; no logic in Subscribe (side-effects via Do and property setters).
/// - Uses PanelChildrenVisibilityMonitor for modular visibility computation.
/// - Maps containers back to data items using container.DataContext.
/// </summary>
public class VisibilityAwareItemsControl : ItemsControl
{
    private readonly CompositeDisposable disposables = new();

    public VisibilityAwareItemsControl()
    {
        var serialDisposable = new SerialDisposable().DisposeWith(disposables);

        Observable
            .Timer(TimeSpan.Zero, TimeSpan.FromTicks(100), AvaloniaScheduler.Instance)
            .Select(l => this.ItemsPanelRoot)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(panel => new ChildWatcher(panel))
            .Do(watcher => serialDisposable.Disposable = watcher)
            .Select(watcher => watcher.VisibleChildren.ToObservableChangeSet())
            .Switch()
            .Transform(visual => ItemFromContainer((Control)visual)!)
            .Bind(out var visibleItems)
            .Subscribe()
            .DisposeWith(disposables);

        VisibleItems = visibleItems;
    }

    public ReadOnlyObservableCollection<object> VisibleItems { get; }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }
}