using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Aggregation;

namespace Zafiro.Avalonia.Misc;

public class ObservableSelectionModel<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public ObservableSelectionModel(SelectionModel<T> selection, Func<T, TKey> selector)
    {
        var cache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable); 
        
        var obs = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => selection.SelectionChanged += handler, handler => selection.SelectionChanged -= handler);

        obs
            .Do(pattern => Sync(pattern.EventArgs, cache))
            .Subscribe()
            .DisposeWith(disposable);

        Selection = cache.Connect(suppressEmptyChangeSets: false);
    }

    public IObservable<IChangeSet<T, TKey>> Selection { get; }

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