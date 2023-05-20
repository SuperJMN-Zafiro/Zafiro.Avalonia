using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Zafiro.UI.Avalonia
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
