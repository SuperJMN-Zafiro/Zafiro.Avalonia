using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class VideoViewAdvanced : NativeControlHost
    {
        public static readonly AvaloniaProperty<string> SourceProperty =
            AvaloniaProperty.Register<VideoViewAdvanced, string>(
                nameof(Source), defaultBindingMode: BindingMode.OneWay);

        public static readonly AvaloniaProperty<TimeSpan> PositionProperty =
            AvaloniaProperty.Register<VideoViewAdvanced, TimeSpan>(
                nameof(Position), defaultBindingMode: BindingMode.TwoWay);

        public static readonly AvaloniaProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<VideoViewAdvanced, TimeSpan>(
                nameof(Duration), defaultBindingMode: BindingMode.TwoWay);

        private readonly CompositeDisposable disposables = new();
        private readonly ObservableMediaPlayer mediaPlayer = new(new(Vlc.Instance));

        public VideoViewAdvanced()
        {
            this.Bind(PositionProperty, mediaPlayer.PositionChanged, BindingPriority.Animation);
            this.Bind(DurationProperty, mediaPlayer.LengthChanged, BindingPriority.Animation);

            this.GetObservable(SourceProperty).Subscribe(LoadMedia);
        }

        private void LoadMedia(string? path)
        {
            if (path == null)
            {
                return;
            }

            mediaPlayer.Media = new Media(Vlc.Instance, new Uri(path));
        }

        public string Source
        {
            get => this.GetValue<string>(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

        public TimeSpan Position
        {
            get => this.GetValue<TimeSpan>(PositionProperty);
            set => this.SetValue(PositionProperty, value);
        }

        public TimeSpan Duration
        {
            get => this.GetValue<TimeSpan>(DurationProperty);
            set => this.SetValue(DurationProperty, value);
        }

        public IObservable<TimeSpan> PositionChanged => mediaPlayer.PositionChanged;

        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            mediaPlayer.Handle = handle;
            return handle;
        }

        /// <inheritdoc />
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            Dispose();
            base.DestroyNativeControlCore(control);
        }

        private void Dispose()
        {
            disposables.Dispose();
        }

        public void SeekTo(TimeSpan pos)
        {
            mediaPlayer.SeekTo(pos);
        }

        public void Play()
        {
            mediaPlayer.Play();
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