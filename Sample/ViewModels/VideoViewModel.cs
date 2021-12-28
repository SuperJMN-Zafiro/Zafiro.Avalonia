using System;
using ReactiveUI;
using Zafiro.Avalonia.LibVLCSharp;

namespace AvaloniaApplication1.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<IMedia> media;
        public string Source { get; }

        public VideoViewModel(string source, MediaFactory mediaFactory)
        {
            Source = source;
            media = mediaFactory
                .CreateFor(this)
                .ToProperty(this, x => x.Media);
        }

        public IMedia Media
        {
            get => media.Value;
        }
    }
}