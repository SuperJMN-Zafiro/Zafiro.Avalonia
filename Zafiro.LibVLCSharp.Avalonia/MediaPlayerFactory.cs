using System.Reactive.Subjects;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class MediaPlayerFactory
    {
        private Dictionary<object, ISubject<IMedia>> subjects = new();

        public MediaPlayerFactory()
        {
            MessageBus.Current.Listen<MediaPlayerCreated>()
                .Subscribe(created =>
                {
                    if (!subjects.ContainsKey(created.ViewDataContext))
                    {
                        return;
                    }

                    subjects[created.ViewDataContext].OnNext(created.Media);
                });
        }

        public IObservable<IMedia> Create(object viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var subject = new Subject<IMedia>();
            subjects[viewModel] = subject;
            return subject;
        }
    }
}