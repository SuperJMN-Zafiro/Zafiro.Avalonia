using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;

namespace Zafiro.Avalonia.Misc;

public class ReactiveSelection<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public ReactiveSelection(Func<T, TKey> selector)
    {
        var cache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable);
        
        SelectionModel = new SelectionModel<T>();
        
        var obs = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => SelectionModel.SelectionChanged += handler, handler => SelectionModel.SelectionChanged -= handler);

        obs
            .Do(pattern => Sync(pattern.EventArgs, cache))
            .Subscribe()
            .DisposeWith(disposable);

        cache.Connect(suppressEmptyChangeSets: false)
            .Bind(out var selectedItems)
            .Subscribe()
            .DisposeWith(disposable);

        SelectedItems = selectedItems;
    }

    public SelectionModel<T> SelectionModel { get; }

    public ReadOnlyObservableCollection<T> SelectedItems { get; }

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