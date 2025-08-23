using System.Collections.Specialized;

namespace Zafiro.Avalonia.Controls;

public class Empty
{
    private static readonly AttachedProperty<NotifyCollectionChangedEventHandler?> HandlerProperty =
        AvaloniaProperty.RegisterAttached<Empty, ItemsControl, NotifyCollectionChangedEventHandler?>("Handler");

    public static readonly AttachedProperty<object?> ContentProperty =
        AvaloniaProperty.RegisterAttached<Empty, Control, object?>("Content");

    public static void SetContent(Visual obj, object? value)
    {
        obj.SetValue(ContentProperty, value);
        if (obj is not ItemsControl items)
            return;

        var handler = items.GetValue(HandlerProperty);
        if (value is null)
        {
            if (handler != null) items.Items.CollectionChanged -= handler;
            items.Classes.Remove("empty");
            return;
        }

        if (handler == null)
        {
            handler = (_, __) => UpdateEmptyClass(items);
            items.SetValue(HandlerProperty, handler);
            items.Items.CollectionChanged += handler;
        }

        UpdateEmptyClass(items);
    }

    public static object? GetContent(Visual obj) => obj.GetValue(ContentProperty);

    private static void UpdateEmptyClass(ItemsControl items) =>
        items.Classes.Set("empty", items.Items.Count == 0);
}
