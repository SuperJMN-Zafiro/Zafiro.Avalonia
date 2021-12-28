using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class MediaFactory
    {
        private readonly Dictionary<object, ISubject<IMedia>> viewModelToMediaDictionary = new(ReferenceEqualityComparer.Instance);

        public MediaFactory()
        {
            MessageBus.Current.Listen<MediaPlayerCreated>()
                .Subscribe(created =>
                {
                    var vm = created.ViewDataContext;
                    var vmFound = viewModelToMediaDictionary.TryFind(vm);
                    vmFound.Execute(subject => subject.OnNext(created.Media));
                });
        }

        public IObservable<IMedia> CreateFor(object viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var sequence = new Subject<IMedia>();
            viewModelToMediaDictionary[viewModel] = sequence;
            return sequence;
        }
    }
}