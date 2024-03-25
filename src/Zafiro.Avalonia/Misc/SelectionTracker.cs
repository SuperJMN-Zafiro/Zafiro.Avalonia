using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Aggregation;

namespace Zafiro.Avalonia.Misc;

public class SelectionTracker<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public SelectionTracker(SelectionModel<T> selection, Func<T, TKey> selector)
    {
        var cache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable); 
        
        var obs = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => selection.SelectionChanged += handler, handler => selection.SelectionChanged -= handler);

        obs
            .Do(pattern => Sync(pattern.EventArgs, cache))
            .Subscribe()
            .DisposeWith(disposable);

        Changes = cache.Connect(suppressEmptyChangeSets: false);
        TotalCount = selection.WhenAnyValue(x => x.Source, selector: enumerable => enumerable?.Cast<object>().Count() ?? 0);
        SelectionCount = Changes.Count();
    }

    public IObservable<int> SelectionCount { get; }

    public IObservable<int> TotalCount { get; }

    public IObservable<IChangeSet<T, TKey>> Changes { get; }

    private static void Sync(SelectionModelSelectionChangedEventArgs<T> pattern, SourceCache<T, TKey> sourceCache)
    {
        sourceCache.Edit(x =>
        {
            x.Remove(pattern.DeselectedItems!);
            x.AddOrUpdate(pattern.SelectedItems!);
        });
    }

    public void Dispose()
    {
        disposable.Dispose();
    }
}