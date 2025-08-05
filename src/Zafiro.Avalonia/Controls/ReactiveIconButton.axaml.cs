using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class ReactiveIconButton : ReactiveButton
{
    public static readonly StyledProperty<object> IconProperty =
        AvaloniaProperty.Register<ReactiveIconButton, object>(nameof(Icon));

    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}

