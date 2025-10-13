using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Aggregation;
using Reactive.Bindings;
using Zafiro.Reactive;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Zafiro.Avalonia.Misc;

public sealed class ReactiveSelection<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public ReactiveSelection(SelectionModel<T> selectionModel, Func<T, TKey> selector, Func<T, bool>? countItemAsSelectable = null)
    {
        var selectedItemsCache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable);

        SelectionModel = selectionModel;
        CountsItemAsSelectable = countItemAsSelectable ?? (_ => true);

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
        var filteredCounts = itemChanges.Filter(CountsItemAsSelectable).Count().CombineLatest(selectedItemsCache.CountChanged, (totalCount, selectedCount) => (totalCount, selectedCount));

        SelectAll = ReactiveCommand.Create(DoSelectAll, filteredCounts.Select(x => x.totalCount > x.selectedCount && x.totalCount > 0)).DisposeWith(disposable);
        Clear = ReactiveCommand.Create(DoClear, counts.Select(x => x.selectedCount > 0)).DisposeWith(disposable);
        SelectedItems = selectedItems;
        SelectedItem = new ReadOnlyReactiveProperty<Maybe<T>>(selectedItems.ToCollection().Select(ts => ts.TryFirst()), Maybe<T>.None).DisposeWith(disposable);
        SelectionCount = new ReadOnlyReactiveProperty<int>(selectedItemsCache.CountChanged).DisposeWith(disposable);
    }

    public ReadOnlyReactiveProperty<int> SelectionCount { get; }
    public IReadOnlyReactiveProperty<Maybe<T>> SelectedItem { get; }
    public ReactiveCommand<Unit, Unit> Clear { get; }
    public ReactiveCommand<Unit, Unit> SelectAll { get; }
    private List<T>? ItemList => SelectionModel.Source?.Cast<T>().ToList();
    public SelectionModel<T> SelectionModel { get; }
    public Func<T, bool> CountsItemAsSelectable { get; }
    public ReadOnlyObservableCollection<T> SelectedItems { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private static IObservable<IChangeSet<T>> ItemChanges(IEnumerable? src)
    {
        return src is IEnumerable<T> source ? source.ToObservableChangeSetIfPossible() : Enumerable.Empty<T>().AsObservableChangeSet();
    }

    private void DoSelectAll()
    {
        if (SelectionModel.SingleSelect)
        {
            return;
        }

        if (SelectionModel.Source is null)
        {
            return;
        }

        var selectedIndices = ItemList?.Where(CountsItemAsSelectable).Select(x => ItemList.IndexOf(x));

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