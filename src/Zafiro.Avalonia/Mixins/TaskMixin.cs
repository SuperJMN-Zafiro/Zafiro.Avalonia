using CSharpFunctionalExtensions;
using System.Reactive;
using System.Reactive.Linq;

namespace Zafiro.Avalonia.Mixins;

public static class TaskMixin
{
    public static async Task<Unit> ToSignal(this Func<Task> task)
    {
        await task();
        return Unit.Default;
    }
}

public static class FunctionalMixin
{
    public static IObservable<Unit> Empties<T>(this IObservable<Maybe<T>> self) => self.Where(x => !x.HasValue).Select(_ => Unit.Default);
}