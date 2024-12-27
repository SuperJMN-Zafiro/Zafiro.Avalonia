using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Services;

internal class DummyNotificationService : INotificationService
{
    public Task Show(string message, Maybe<string> title)
    {
        return Task.CompletedTask;
    }
}