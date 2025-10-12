using Avalonia.Styling;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls;

public class ReactiveButton : ContentControl
{
    public static readonly StyledProperty<IEnhancedCommand> CommandProperty = AvaloniaProperty.Register<ReactiveButton, IEnhancedCommand>(
        nameof(Command));

    public static readonly StyledProperty<ControlTheme> ButtonThemeProperty = AvaloniaProperty.Register<ReactiveButton, ControlTheme>(
        nameof(ButtonTheme));

    public static readonly StyledProperty<bool> IsCancelProperty = AvaloniaProperty.Register<ReactiveButton, bool>(
        nameof(IsCancel));

    public static readonly StyledProperty<bool> IsDefaultProperty = AvaloniaProperty.Register<ReactiveButton, bool>(
        nameof(IsDefault));

    public IEnhancedCommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public ControlTheme ButtonTheme
    {
        get => GetValue(ButtonThemeProperty);
        set => SetValue(ButtonThemeProperty, value);
    }

    public bool IsCancel
    {
        get => GetValue(IsCancelProperty);
        set => SetValue(IsCancelProperty, value);
    }

    public bool IsDefault
    {
        get => GetValue(IsDefaultProperty);
        set => SetValue(IsDefaultProperty, value);
    }
}