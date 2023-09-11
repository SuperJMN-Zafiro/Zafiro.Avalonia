using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.MigrateToZafiro;

public static class FunctionalMixin
{
    public static IObservable<Unit> Empties<T>(this IObservable<Maybe<T>> self) => self.Where(x => !x.HasValue).Select(_ => Unit.Default);
}