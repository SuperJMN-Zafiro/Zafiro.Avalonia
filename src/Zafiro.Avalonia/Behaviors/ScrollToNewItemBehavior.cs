using System.Collections.Specialized;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
using DynamicData.Binding;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Behaviors;

[PublicAPI]
public class ScrollToNewItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var itemCollectionChanges = this.WhenAnyValue(x => x.AssociatedObject, x => x.AssociatedObject!.DataContext, x => x.AssociatedObject!.Items, (associateControl, _, _) => associateControl)
            .WhereNotNull()
            .Select(x => x.Items as INotifyCollectionChanged)
            .WhereNotNull()
            .Select(incc => incc.ObserveCollectionChanges())
            .Switch();

        var newItem = itemCollectionChanges
            .Where(pattern => pattern.EventArgs.Action == NotifyCollectionChangedAction.Add)
            .Select(eventPattern => eventPattern.EventArgs.NewItems?.Cast<object>().FirstOrDefault())
            .WhereNotNull();

        return newItem
            .Do(ScrollTo)
            .Subscribe();
    }

    private void ScrollTo(object obj)
    {
        // Avalonia doesn't provide us with any method to scroll to a given item, so we just scroll to end.
        var scrollViewer = AssociatedObject.FindDescendantOfType<ScrollViewer>() ?? AssociatedObject.FindAncestorOfType<ScrollViewer>() ?? throw new InvalidOperationException("We can't find any ScrollViewer we can scroll.");
        scrollViewer.ScrollToEnd();
    }
}