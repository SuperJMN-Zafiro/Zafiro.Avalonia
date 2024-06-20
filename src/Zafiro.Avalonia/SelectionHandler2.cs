using System.Reactive;
using Avalonia.Controls.Selection;
using DynamicData;
using Zafiro.Avalonia.Misc;
using Zafiro.Reactive;

namespace Zafiro.Avalonia;

public class SelectionHandler2<T, TKey> : ReactiveObject, ISelectionHandler<T, TKey> where TKey : notnull where T : notnull
{
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectAll;
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectNone;

    public SelectionHandler2(IObservable<SelectionModel<T>> selectionModels, Func<T, TKey> keySelector)
    {
        var trackers = selectionModels.Select(x => new ObservableSelectionModel<T, TKey>(x, keySelector)).DisposePrevious();

        TotalCount = trackers.Select(x => x.TotalCount).Switch();
        SelectionChanges = trackers.Select(x => x.Selection).Switch();
        
        selectAll = selectionModels.Select(x => ReactiveCommand.Create(x.SelectAll)).ToProperty(this, handler => handler.SelectAll);
        selectNone = selectionModels.Select(x => ReactiveCommand.Create(x.Clear)).ToProperty(this, handler => handler.SelectNone);

        var selectionCount = trackers.Select(tracker => tracker.Selection.Count().StartWith(0)).Switch();
        SelectionCount = selectionCount;
    }

    public IObservable<int> TotalCount { get; }
    public IObservable<IChangeSet<T, TKey>> SelectionChanges { get; }
    public ReactiveCommand<Unit, Unit> SelectNone => selectNone.Value;
    public ReactiveCommand<Unit, Unit> SelectAll => selectAll.Value;
    public IObservable<int> SelectionCount { get; }
}