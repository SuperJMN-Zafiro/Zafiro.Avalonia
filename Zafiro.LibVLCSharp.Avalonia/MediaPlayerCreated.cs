namespace Zafiro.Avalonia.LibVLCSharp;

public class MediaPlayerCreated
{
    public IMedia Media { get; }
    public object? ViewDataContext { get; }

    public MediaPlayerCreated(IMedia media, object viewDataContext)
    {
        Media = media;
        ViewDataContext = viewDataContext;
    }
}