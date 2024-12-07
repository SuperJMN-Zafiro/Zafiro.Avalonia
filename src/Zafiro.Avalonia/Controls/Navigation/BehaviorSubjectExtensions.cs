using System.Reactive.Subjects;

namespace Zafiro.Avalonia.Controls.Navigation;

public static class BehaviorSubjectExtensions
{
    public static IBehaviorSubject<T> AsBehaviorSubject<T>(this BehaviorSubject<T> subject) =>
        new BehaviorSubjectWrapper<T>(subject);

    private class BehaviorSubjectWrapper<T> : IBehaviorSubject<T>
    {
        private readonly BehaviorSubject<T> subject;

        public BehaviorSubjectWrapper(BehaviorSubject<T> subject)
        {
            this.subject = subject;
        }

        public T Value => subject.Value;

        public IDisposable Subscribe(IObserver<T> observer) => subject.Subscribe(observer);
    }
}