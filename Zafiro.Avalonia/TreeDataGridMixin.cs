using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Core.Trees;

namespace Zafiro.Avalonia;

public static class TreeDataGridMixin
{
    public static int ModelToRowIndex<T>(this HierarchicalTreeDataGridSource<T> source, T model, Func<T, IEnumerable<T>?> getChildren)
    {
        var modelPath = source.Items.GetPath(model, getChildren);
        var modelIndex = new IndexPath(modelPath);
        return source.Rows.ModelIndexToRowIndex(modelIndex);
    }
    
    public static void BringIntoView<T>(this TreeDataGrid treeDataGrid, T model, Func<T, IEnumerable<T>> getChildren)
    {
        if (treeDataGrid is { RowsPresenter: { Items: { } } rowsPresenter, Source: HierarchicalTreeDataGridSource<T> source })
        {
            var modelPath = source.Items.GetPath(model, getChildren);
            ExpandPath<T>(source, modelPath);

            var index = ModelToRowIndex(source, model, getChildren);
            
            rowsPresenter.BringIntoView(index);
        }
    }

    public static void ExpandPath<T>(this ITreeDataGridSource source, IEnumerable<int> modelPath)
    {
        var paths = Grow(modelPath);

        foreach (var path in paths.SkipLast(1))
        {
            var rowId = source.Rows.ModelIndexToRowIndex(new IndexPath(path));
            var row = (HierarchicalRow<T>) source.Rows[rowId];
            row.IsExpanded = true;
        }
    }

    public static IEnumerable<IEnumerable<T>> Grow<T>(IEnumerable<T> sequence)
    {
        return sequence.Select((x, i) => sequence.Take(i + 1));
    }
}