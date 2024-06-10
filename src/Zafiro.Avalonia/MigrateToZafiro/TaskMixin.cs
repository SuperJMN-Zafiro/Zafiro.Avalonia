using System.Reactive;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

namespace Zafiro.Avalonia.MigrateToZafiro;

public static class TaskMixin
{
    public static async Task<Unit> ToSignal(this Func<Task> task)
    {
        await task().ConfigureAwait(false);
        return Unit.Default;
    }

    public static Maybe<T> IgnoreResult<T>(this Result<Maybe<T>> resultOfMaybe)
    {
        return resultOfMaybe.Match(maybe => maybe, _ => Maybe<T>.None);
    }
}