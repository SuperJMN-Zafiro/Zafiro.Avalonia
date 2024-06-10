namespace Zafiro.Avalonia.Controls;

public class Empty : AvaloniaObject
{
    public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Empty, Control, object>("Content");

    public static void SetContent(Control obj, object value) => obj.SetValue(ContentProperty, value);
    public static object GetContent(Control obj) => obj.GetValue(ContentProperty);
}