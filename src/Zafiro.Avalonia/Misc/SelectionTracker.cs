using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;

namespace Zafiro.Avalonia.Misc;

public interface ISelectionHandler<T, TKey> where T : notnull where TKey : notnull
{
    IObservable<IChangeSet<T, TKey>> Changes { get; }
    ReactiveCommand<Unit, Unit> SelectNone { get; }
    ReactiveCommand<Unit, Unit> SelectAll { get; }
}

public class SelectionHandler<T, TKey> : ISelectionHandler<T, TKey> where T : notnull where TKey : notnull
{
    public SelectionHandler(SelectionModel<T> model, Func<T, TKey> selector)
    {
        SelectAll = ReactiveCommand.Create(() => model.SelectAll());
        SelectNone = ReactiveCommand.Create(() => model.Clear());
        var tracker = new SelectionTracker<T, TKey>(model, selector);
        Changes = tracker.Changes;
    }

    public IObservable<IChangeSet<T, TKey>> Changes { get; }

    public ReactiveCommand<Unit, Unit> SelectNone { get; }

    public ReactiveCommand<Unit, Unit> SelectAll { get; }
}

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

        Changes = cache.Connect();
    }

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