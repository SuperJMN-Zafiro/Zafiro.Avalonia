using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors;

public class SelectAllAction : StyledElementAction
{
    [ResolveByName]
    public object? ListBox { get; set; } = null!;

    public override object? Execute(object? sender, object? parameter)
    {
        if (ListBox is ListBox listBox)
        {
            var selectedIndices = GetSelectedIndices(listBox);

            foreach (var index in selectedIndices)
            {
                listBox.Selection.Select(index);    
            }
        }

        return true;
    }

    private IEnumerable<int> GetSelectedIndices(ListBox listBox)
    {
        return listBox.Items.Where(o => o != null)
            .Select(o => listBox.ContainerFromItem(o!))
            .Where(control => control != null && control is ListBoxItem lbi && (CanSelectDisabled || lbi.IsEnabled))
            .Select(cont => listBox.IndexFromContainer(cont!));
    }

    public bool CanSelectDisabled { get; set; } = false;
}