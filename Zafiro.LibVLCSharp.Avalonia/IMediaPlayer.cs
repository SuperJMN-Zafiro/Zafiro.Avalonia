namespace Zafiro.Avalonia.LibVLCSharp
{
    public interface IMediaPlayer
    {
        void Play();
        void SeekTo(TimeSpan beginning);
        IObservable<TimeSpan> Position { get; }
    }
}