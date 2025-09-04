using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.ViewLocators;

public class ReactiveSelection<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public ReactiveSelection(SelectionModel<T> selectionModel, Func<T, TKey> selector, Func<T, bool>? canBeSelected = null)
    {
        var selectedItemsCache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable);

        SelectionModel = selectionModel;
        CanBeSelected = canBeSelected ?? (_ => true);

        var selectionChanged = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => SelectionModel.SelectionChanged += handler, handler => SelectionModel.SelectionChanged -= handler);

        selectionChanged
            .Do(pattern => Sync(pattern.EventArgs, selectedItemsCache))
            .Subscribe()
            .DisposeWith(disposable);

        selectedItemsCache.Connect(suppressEmptyChangeSets: false)
            .Bind(out var selectedItems)
            .Subscribe()
            .DisposeWith(disposable);

        var itemChanges = this.WhenAnyValue(x => x.SelectionModel.Source).Select(ItemChanges).Switch();

        var counts = itemChanges.Count().CombineLatest(selectedItemsCache.CountChanged, (totalCount, selectedCount) => (totalCount, selectedCount));
        var filteredCounts = itemChanges.Filter(CanBeSelected).Count().CombineLatest(selectedItemsCache.CountChanged, (totalCount, selectedCount) => (totalCount, selectedCount));

        SelectAll = ReactiveCommand.Create(DoSelectAll, filteredCounts.Select(x => x.totalCount > x.selectedCount && x.totalCount > 0)).DisposeWith(disposable);
        Clear = ReactiveCommand.Create(DoClear, counts.Select(x => x.selectedCount > 0)).DisposeWith(disposable);

        SelectedItems = selectedItems;
    }

    public ReactiveCommand<Unit, Unit> Clear { get; }

    public ReactiveCommand<Unit, Unit> SelectAll { get; }

    private IList<T>? ItemList => SelectionModel.Source?.Cast<T>().ToList();

    public SelectionModel<T> SelectionModel { get; }
    public Func<T, bool> CanBeSelected { get; }

    public ReadOnlyObservableCollection<T> SelectedItems { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private static IObservable<IChangeSet<T>> ItemChanges(IEnumerable? src)
    {
        return src is IEnumerable<T> source ? source.ToObservableChangeSetIfPossible() : Enumerable.Empty<T>().AsObservableChangeSet();
    }

    public void DoSelectAll()
    {
        if (SelectionModel.SingleSelect)
        {
            return;
        }

        if (SelectionModel.Source is null)
        {
            return;
        }

        var selectedIndices = ItemList?.Where(CanBeSelected).Select(x => ItemList.IndexOf(x));

        if (selectedIndices is null)
        {
            return;
        }

        foreach (var selectedItem in selectedIndices) SelectionModel.Select(selectedItem);
    }

    private void DoClear()
    {
        SelectionModel.Clear();
    }

    private static void Sync(SelectionModelSelectionChangedEventArgs<T> pattern, SourceCache<T, TKey> sourceCache)
    {
        sourceCache.Edit(x =>
        {
            x.Remove(pattern.DeselectedItems!);
            x.AddOrUpdate(pattern.SelectedItems!);
        });
    }
}