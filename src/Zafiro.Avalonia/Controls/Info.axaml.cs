using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls;

public class Info : TemplatedControl
{
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<Info, object>(nameof(Icon));

    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<object> DetailsProperty = AvaloniaProperty.Register<Info, object>(nameof(Details));

    public object Details
    {
        get => GetValue(DetailsProperty);
        set => SetValue(DetailsProperty, value);
    }
}