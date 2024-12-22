using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs;

public class DialogViewContainer : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<DialogViewContainer, string>(
        nameof(Title));

    public static readonly StyledProperty<ICommand> CloseProperty = AvaloniaProperty.Register<DialogViewContainer, ICommand>(
        nameof(Close));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ICommand Close
    {
        get => GetValue(CloseProperty);
        set => SetValue(CloseProperty, value);
    }
}