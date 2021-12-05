using System.Reactive.Subjects;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class MediaPlayerFactory
    {
        private Dictionary<object, ISubject<IMediaPlayer>> subjects = new();

        public MediaPlayerFactory()
        {
            MessageBus.Current.Listen<MediaPlayerCreated>()
                .Subscribe(created =>
                {
                    if (!subjects.ContainsKey(created.ViewDataContext))
                    {
                        return;
                    }

                    subjects[created.ViewDataContext].OnNext(created.MediaPlayer);
                });
        }

        public IObservable<IMediaPlayer> Create(object viewModel)
        {
            var subject = new Subject<IMediaPlayer>();
            subjects[viewModel] = subject;
            return subject;
        }
    }
}