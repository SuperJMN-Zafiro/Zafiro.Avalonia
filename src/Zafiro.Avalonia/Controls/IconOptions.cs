namespace Zafiro.Avalonia.Controls;

public class IconOptions
{
    public static readonly AttachedProperty<double> SizeProperty = AvaloniaProperty.RegisterAttached<IconOptions, AvaloniaObject, double>("Size", inherits: true, defaultValue: 32);

    public static void SetSize(AvaloniaObject obj, double value) => obj.SetValue(SizeProperty, value);
    public static double GetSize(AvaloniaObject obj) => obj.GetValue(SizeProperty);
}