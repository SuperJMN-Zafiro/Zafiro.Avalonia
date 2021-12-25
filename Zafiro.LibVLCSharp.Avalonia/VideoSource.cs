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

    private sealed class StrEqualityComparer : IEqualityComparer<VideoSource>
    {
        public bool Equals(VideoSource x, VideoSource y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.str == y.str;
        }

        public int GetHashCode(VideoSource obj)
        {
            return obj.str.GetHashCode();
        }
    }

    public static IEqualityComparer<VideoSource> StrComparer { get; } = new StrEqualityComparer();
}