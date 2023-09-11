using System.Reactive;

namespace Zafiro.Avalonia.MigrateToZafiro;

public static class TaskMixin
{
    public static async Task<Unit> ToSignal(this Func<Task> task)
    {
        await task();
        return Unit.Default;
    }
}