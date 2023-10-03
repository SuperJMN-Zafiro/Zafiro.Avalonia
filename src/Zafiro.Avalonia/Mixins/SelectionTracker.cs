using System;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using DynamicData;

namespace Zafiro.Avalonia.Mixins;

public class SelectionTracker<T, TKey> where TKey : notnull
{
    public SelectionTracker(SelectionModel<T> selection, Func<T, TKey> selector)
    {
        var cache = new SourceCache<T, TKey>(selector);

        var obs = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => selection.SelectionChanged += handler, handler => selection.SelectionChanged -= handler);

        obs
            .Do(pattern => Sync(pattern.EventArgs, cache))
            .Subscribe();

        Changes = cache.Connect();
    }

    public IObservable<IChangeSet<T, TKey>> Changes { get; }

    private void Sync(SelectionModelSelectionChangedEventArgs<T> pattern, SourceCache<T, TKey> sourceCache)
    {
        sourceCache.Edit(x =>
        {
            x.Remove(pattern.DeselectedItems!);
            x.AddOrUpdate(pattern.SelectedItems!);
        });
    }
}