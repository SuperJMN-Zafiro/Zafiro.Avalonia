using System.ComponentModel.DataAnnotations;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public interface IMedia
    {
        void Play();
        void SeekTo(TimeSpan beginning);
        IObservable<TimeSpan> Position { get; }
        TimeSpan Duration { get; }
        void Stop();
        void Pause();
    }
}