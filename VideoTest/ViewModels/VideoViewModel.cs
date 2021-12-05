using System;
using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.LibVLCSharp;

namespace VideoTest.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        private IMediaPlayer mediaPlayer;
        public string Source { get; }

        public VideoViewModel(string source, MediaPlayerFactory mediaPlayerFactory)
        {
            Source = source;
            mediaPlayerFactory.Create(this)
                .Subscribe(player =>
            {
                mediaPlayer = player;
            });

            Play = ReactiveCommand.Create(() => mediaPlayer.Play());
        }

        public ReactiveCommand<Unit, Unit> Play { get; set; }
    }
}