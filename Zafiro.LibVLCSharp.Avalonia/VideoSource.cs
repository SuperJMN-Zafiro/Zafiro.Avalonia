using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp;

public class VideoSource
{
    private readonly string str;

    public VideoSource(string str)
    {
        this.str = str;
    }

    public void Setup(MediaPlayer vlcMediaPlayer)
    {
        var media = new Media(Vlc.Instance, new Uri(str));
        Duration = media.Duration;
        vlcMediaPlayer.Media = media;
    }

    public long Duration { get; private set; }
}