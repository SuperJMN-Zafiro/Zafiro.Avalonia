using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

[Obsolete("Replaced by EnhancedButton")]
public class IconButton : Button
{
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<IconButton, object>(nameof(Icon));

    public static readonly StyledProperty<ControlTheme> ButtonThemeProperty = AvaloniaProperty.Register<IconButton, ControlTheme>(
        nameof(ButtonTheme));

    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ControlTheme ButtonTheme
    {
        get => GetValue(ButtonThemeProperty);
        set => SetValue(ButtonThemeProperty, value);
    }
}