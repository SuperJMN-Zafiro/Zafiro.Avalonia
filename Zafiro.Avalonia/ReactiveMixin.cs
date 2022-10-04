using System.Reactive;
using System.Reactive.Linq;

namespace Zafiro.Avalonia;

public static class ReactiveMixin
{
    public static IObservable<Unit> ToSignal<T>(this IObservable<T> source) => source.Select(_ => Unit.Default);

    public static IObservable<T> ReplayLastActive<T>(this IObservable<T> observable) => observable.Replay(1).RefCount();
}