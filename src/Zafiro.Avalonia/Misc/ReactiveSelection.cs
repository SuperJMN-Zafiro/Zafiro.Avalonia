using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Misc;

public class ReactiveSelection<T, TKey> : IDisposable where T : notnull where TKey : notnull
{
    private readonly CompositeDisposable disposable = new();

    public ReactiveSelection(SelectionModel<T> selectionModel, Func<T, TKey> selector, Func<T, bool>? canBeSelected = null)
    {
        var selectedItemsCache = new SourceCache<T, TKey>(selector)
            .DisposeWith(disposable);
        
        SelectionModel = selectionModel;
        CanBeSelected = canBeSelected ?? (_ => true);

        var obs = Observable
            .FromEventPattern<SelectionModelSelectionChangedEventArgs<T>>(handler => SelectionModel.SelectionChanged += handler, handler => SelectionModel.SelectionChanged -= handler);

        obs
            .Do(pattern => Sync(pattern.EventArgs, selectedItemsCache))
            .Subscribe()
            .DisposeWith(disposable);

        selectedItemsCache.Connect(suppressEmptyChangeSets: false)
            .Bind(out var selectedItems)
            .Subscribe()
            .DisposeWith(disposable);
        
        var itemChanges = this.WhenAnyValue(x => x.SelectionModel.Source).Select(src => GetItemChanges(src)).Switch();
        
        var counts = itemChanges.Count().CombineLatest(selectedItemsCache.CountChanged, (totalCount, selectedCount) => (totalCount, selectedCount));
        var filteredCounts = itemChanges.Filter(CanBeSelected).Count().CombineLatest(selectedItemsCache.CountChanged, (totalCount, selectedCount) => (totalCount, selectedCount));

        SelectAllCommand = ReactiveCommand.Create(() => SelectAll(), counts.Select(x => x.totalCount > x.selectedCount && x.totalCount > 0));
        SelectAllFilteredCommand = ReactiveCommand.Create(() => SelectFiltered(), filteredCounts.Select(x => x.totalCount > x.selectedCount && x.totalCount > 0));
        ClearCommand = ReactiveCommand.Create(() => Clear(), counts.Select(x => x.selectedCount > 0));

        SelectedItems = selectedItems;
    }

    public ReactiveCommand<Unit, Unit> ClearCommand { get; set; }

    public ReactiveCommand<Unit, Unit> SelectAllFilteredCommand { get; }

    private static IObservable<IChangeSet<T>> GetItemChanges(IEnumerable? src)
    {
        return src is IEnumerable<T> source ? source.ToObservableChangeSetIfPossible() : Enumerable.Empty<T>().AsObservableChangeSet();
    }

    public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }

    public void SelectAll()
    {
        if (SelectionModel.SingleSelect)
        {
            return;
        }
        
        SelectionModel.SelectAll();
    }
    
    public void SelectFiltered()
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

        foreach (var selectedItem in selectedIndices)
        {
            SelectionModel.Select(selectedItem);   
        }
    }
    
    public IList<T>? ItemList => SelectionModel.Source?.Cast<T>().ToList(); 
    
    public void Clear()
    {
        SelectionModel.Clear();
    }

    public SelectionModel<T> SelectionModel { get; }
    public Func<T, bool> CanBeSelected { get; }

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