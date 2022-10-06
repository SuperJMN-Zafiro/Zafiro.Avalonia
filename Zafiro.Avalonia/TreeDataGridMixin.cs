using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Core.Trees;

namespace Zafiro.Avalonia;

public static class TreeDataGridMixin
{
    public static void BringIntoView(this TreeDataGrid treeDataGrid, object model)
    {
        if (treeDataGrid is { RowsPresenter: { Items: { } } rowsPresenter, Source: { } source })
        {
            var modelPath = source.Items.GetPath(model, source.GetModelChildren);
            ExpandPath(source, modelPath);

            var index = ModelToRowIndex(source, model);
            
            rowsPresenter.BringIntoView(index);
        }
    }

    public static int ModelToRowIndex(this ITreeDataGridSource source, object model)
    {
        var modelPath = source.Items.GetPath(model, source.GetModelChildren);
        var modelIndex = new IndexPath(modelPath);
        return source.Rows.ModelIndexToRowIndex(modelIndex);
    }

    public static void ExpandPath(this ITreeDataGridSource source, IEnumerable<int> modelPath)
    {
        var paths = Grow(modelPath);

        foreach (var path in paths.SkipLast(1))
        {
            var rowId = source.Rows.ModelIndexToRowIndex(new IndexPath(path));
            var row = (IExpander) source.Rows[rowId];
            row.IsExpanded = true;
        }
    }

    public static IEnumerable<IEnumerable<T>> Grow<T>(IEnumerable<T> sequence)
    {
        return sequence.Select((x, i) => sequence.Take(i + 1));
    }
}