using System.Reactive;
using Avalonia.Controls.Selection;
using DynamicData;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia;

public class SelectionHandler<T, TKey> : ISelectionHandler<T, TKey> where T : notnull where TKey : notnull
{
    public SelectionHandler(SelectionModel<T> selectionModel, Func<T, TKey> selector)
    {
        var tracker = new SelectionTracker<T, TKey>(selectionModel, selector);
        SelectAll = ReactiveCommand.Create(selectionModel.SelectAll);
        SelectNone = ReactiveCommand.Create(selectionModel.Clear);
        SelectionCount = tracker.SelectionCount;
        TotalCount = tracker.TotalCount;
        SelectionChanges = tracker.Changes;
    }

    public ReactiveCommand<Unit, Unit> SelectNone { get; }
    public ReactiveCommand<Unit, Unit> SelectAll { get; }
    public IObservable<int> SelectionCount { get; }
    public IObservable<int> TotalCount { get; }
    public IObservable<IChangeSet<T, TKey>> SelectionChanges { get; }
}