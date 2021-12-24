using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class MediaPlayerFactory
    {
        private Dictionary<object, ISubject<IMedia>> subjects = new Dictionary<object, ISubject<IMedia>>(ReferenceEqualityComparer.Instance);

        public MediaPlayerFactory()
        {
            MessageBus.Current.Listen<MediaPlayerCreated>()
                .Subscribe(created =>
                {
                    var vm = created.ViewDataContext;
                    var vmFound = subjects.TryFind(vm);
                    vmFound.Execute(subject => subject.OnNext(created.Media));
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