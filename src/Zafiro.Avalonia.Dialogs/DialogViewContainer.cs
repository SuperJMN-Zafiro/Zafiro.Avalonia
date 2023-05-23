using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs
{
    public class DialogViewContainer : ContentControl
    {
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<DialogViewContainer, string>(
            "Title");

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
