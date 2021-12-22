using System.Reactive.Linq;
using Avalonia.Platform;
using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public interface IVlcPlayer
    {
        public Media Media { get; set; }
    }

    public class VlcMediaPlayer : IDisposable
    {
        private readonly MediaPlayer mediaPlayer;
        private IPlatformHandle handle;

        public VlcMediaPlayer(MediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;
            LengthChanged = Observable
                .FromEventPattern<MediaPlayerLengthChangedEventArgs>(h => mediaPlayer.LengthChanged += h,
                    h => mediaPlayer.LengthChanged -= h)
                .Select(a => TimeSpan.FromMilliseconds(a.EventArgs.Length));

            PositionChanged = Observable
                .FromEventPattern<MediaPlayerTimeChangedEventArgs>(h => mediaPlayer.TimeChanged += h,
                    h => mediaPlayer.TimeChanged -= h)
                .Select(a => TimeSpan.FromMilliseconds(a.EventArgs.Time));
        }

        public IObservable<TimeSpan> PositionChanged { get; }

        public IObservable<TimeSpan> LengthChanged { get; }

        public IPlatformHandle Handle
        {
            get => handle;
            set
            {
                handle = value;
                mediaPlayer.SetHandle(value);
            }
        }

        public void Play()
        {
            mediaPlayer.Play();
        }

        public void Dispose()
        {
            mediaPlayer.DisposeHandle();
            mediaPlayer.Dispose();
        }

        public void SeekTo(TimeSpan pos)
        {
            mediaPlayer.SeekTo(pos);
        }

        public void Pause()
        {
            mediaPlayer.Pause();
        }

        public void Stop()
        {
            mediaPlayer.Stop();
        }
    }
}