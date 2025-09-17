namespace Zafiro.Avalonia.Controls;

public class IconOptions
{
    public static readonly AttachedProperty<double> SizeProperty = AvaloniaProperty.RegisterAttached<IconOptions, Visual, double>("Size", inherits: true, defaultValue: 32);

    public static void SetSize(Visual obj, double value) => obj.SetValue(SizeProperty, value);
    public static double GetSize(Visual obj) => obj.GetValue(SizeProperty);
}