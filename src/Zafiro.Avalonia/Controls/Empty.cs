namespace Zafiro.Avalonia.Controls;

public class Empty : AvaloniaObject
{
    public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Empty, Control, object>("Content");

    public static void SetContent(Visual obj, object value) => obj.SetValue(ContentProperty, value);
    public static object GetContent(Visual obj) => obj.GetValue(ContentProperty);
}