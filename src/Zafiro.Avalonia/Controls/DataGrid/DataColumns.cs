using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Zafiro.Avalonia.Controls.DataGrid;

public class DataColumns : ObservableCollection<DataColumn>
{
    protected override void InsertItem(int index, DataColumn item)
    {
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, DataColumn item)
    {
        base.SetItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        base.RemoveItem(index);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
        base.MoveItem(oldIndex, newIndex);
    }

    protected override void ClearItems()
    {
        base.ClearItems();
    }
}