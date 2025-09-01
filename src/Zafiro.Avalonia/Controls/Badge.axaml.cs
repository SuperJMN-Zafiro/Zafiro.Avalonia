using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

public class Badge : ContentControl
{
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<Badge, Color>(
        nameof(Color));

    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}