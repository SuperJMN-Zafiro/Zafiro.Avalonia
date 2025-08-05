using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Controls;

public class Empty : AvaloniaObject
{
    const string EmptyClass = "empty";

    public static readonly AttachedProperty<object> ContentProperty =
        AvaloniaProperty.RegisterAttached<Empty, Control, object>("Content", "Nothing to show");

    static readonly AttachedProperty<IDisposable?> SubscriptionProperty =
        AvaloniaProperty.RegisterAttached<Empty, Control, IDisposable?>("Subscription");

    static Empty()
    {
        ContentProperty.Changed.Subscribe(OnContentChanged);
    }

    static void OnContentChanged(AvaloniaPropertyChangedEventArgs<object?> change)
    {
        if (change.Sender is not ItemsControl itemsControl)
        {
            return;
        }

        Maybe<IDisposable>.From(itemsControl.GetValue(SubscriptionProperty)).Execute(d => d.Dispose());

        var items = itemsControl.Items;
        var observable = Observable
            .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => items.CollectionChanged += h,
                h => items.CollectionChanged -= h);

        var subscription = observable.Subscribe(_ => Update(itemsControl));
        itemsControl.SetValue(SubscriptionProperty, subscription);

        Update(itemsControl);
    }

    static void Update(ItemsControl control)
    {
        if (control.Items.Count == 0)
        {
            control.Classes.Add(EmptyClass);
        }
        else
        {
            control.Classes.Remove(EmptyClass);
        }
    }

    public static void SetContent(Visual obj, object value) => obj.SetValue(ContentProperty, value);
    public static object GetContent(Visual obj) => obj.GetValue(ContentProperty);
}
