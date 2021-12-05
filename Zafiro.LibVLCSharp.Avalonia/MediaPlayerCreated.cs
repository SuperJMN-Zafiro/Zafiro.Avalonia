namespace Zafiro.Avalonia.LibVLCSharp;

public class MediaPlayerCreated
{
    public IMediaPlayer MediaPlayer { get; }
    public object? ViewDataContext { get; }

    public MediaPlayerCreated(IMediaPlayer mediaPlayer, object viewDataContext)
    {
        MediaPlayer = mediaPlayer;
        ViewDataContext = viewDataContext;
    }
}