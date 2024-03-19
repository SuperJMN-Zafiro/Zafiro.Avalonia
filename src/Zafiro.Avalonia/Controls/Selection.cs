namespace Zafiro.Avalonia.Controls;

public class Selection : AvaloniaObject
{
    public static readonly AttachedProperty<bool> EnableCheckBoxesProperty =
        AvaloniaProperty.RegisterAttached<Selection, ListBox, bool>("EnableCheckBoxes");

    public static void SetEnableCheckBoxes(ListBox obj, bool value) => obj.SetValue(EnableCheckBoxesProperty, value);
    public static bool EnableCheckBoxes(ListBox obj) => obj.GetValue(EnableCheckBoxesProperty);
}