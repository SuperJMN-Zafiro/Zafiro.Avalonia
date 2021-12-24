using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using CSharpFunctionalExtensions;
using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class VideoView : NativeControlHost, IMedia
    {
        public static readonly AvaloniaProperty<Maybe<VideoSource>> SourceProperty =
            AvaloniaProperty.Register<VideoView, Maybe<VideoSource>>(
                nameof(Source), defaultBindingMode: BindingMode.OneWay);

        public static readonly AvaloniaProperty<TimeSpan> PositionProperty =
            AvaloniaProperty.Register<VideoView, TimeSpan>(
                nameof(Position), defaultBindingMode: BindingMode.TwoWay);

        public static readonly AvaloniaProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<VideoView, TimeSpan>(
                nameof(Duration), defaultBindingMode: BindingMode.TwoWay);

        public static readonly AvaloniaProperty<bool> AutoPlayProperty =
            AvaloniaProperty.Register<VideoView, bool>(
                nameof(AutoPlay), defaultBindingMode: BindingMode.OneWay);

        private readonly CompositeDisposable disposables = new();
        private readonly MediaPlayer mediaPlayer = new(Vlc.Instance);
        private readonly IObservable<TimeSpan> positionChanged;
        private readonly IObservable<TimeSpan> lengthChanged;

        public VideoView()
        {
            positionChanged = mediaPlayer.GetPositionChanged();
            lengthChanged = mediaPlayer.GetLengthChanged();

            this.Bind(DurationProperty, lengthChanged, BindingPriority.Animation);
            this.Bind(PositionProperty, positionChanged, BindingPriority.Animation);

            this.GetObservable(SourceProperty).Subscribe(LoadMedia);
        }

        private void LoadMedia(Maybe<VideoSource> source)
        {
            source.Execute(v =>
            {
                v.Setup(mediaPlayer);
                if (AutoPlay)
                {
                    mediaPlayer.Play();
                }
            });
        }

        public Maybe<VideoSource> Source
        {
            get => this.GetValue<Maybe<VideoSource>>(SourceProperty);
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

        public bool AutoPlay
        {
            get => this.GetValue<bool>(AutoPlayProperty);
            set => this.SetValue(AutoPlayProperty, value);
        }

        public IObservable<TimeSpan> PositionChanged => positionChanged;
        public IObservable<TimeSpan> DurationChanged => lengthChanged;

        IObservable<TimeSpan> IMedia.Position => mediaPlayer.GetPositionChanged();

        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            mediaPlayer.SetHandle(handle);
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