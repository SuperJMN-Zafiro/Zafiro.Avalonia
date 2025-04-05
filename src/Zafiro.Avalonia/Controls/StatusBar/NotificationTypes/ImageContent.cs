namespace Zafiro.Avalonia.Controls.StatusBar.NotificationTypes;

public class ImageContent : ReactiveObject
{
    public ImageContent(Uri source)
    {
        Source = source;
    }

    public Uri Source { get; }
}