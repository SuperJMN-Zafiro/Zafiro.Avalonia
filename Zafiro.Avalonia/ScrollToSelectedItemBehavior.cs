using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Zafiro.Avalonia;

public class ScrollToSelectedItemBehavior : Behavior<TreeDataGrid>
{
    private readonly CompositeDisposable disposables = new();

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is { SelectionInteraction: { } selection, RowSelection: { } rowSelection })
        {
            Observable.FromEventPattern(selection, nameof(selection.SelectionChanged))
                .Select(_ => rowSelection.SelectedIndex.FirstOrDefault())
                .WhereNotNull()
                .Do(ScrollToItemIndex)
                .Subscribe()
                .DisposeWith(disposables);
        }
    }

    protected override void OnDetachedFromVisualTree()
    {
        disposables.Dispose();
        base.OnDetachedFromVisualTree();
    }

    private void ScrollToItemIndex(int index)
    {
        if (AssociatedObject is { RowsPresenter: { } rowsPresenter })
        {
            rowsPresenter.BringIntoView(index);
        }
    }
}