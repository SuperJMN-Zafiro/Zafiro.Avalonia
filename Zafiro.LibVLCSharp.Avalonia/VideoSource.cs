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

    protected bool Equals(VideoSource other)
    {
        return str == other.str;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((VideoSource) obj);
    }

    public override int GetHashCode()
    {
        return str.GetHashCode();
    }
}