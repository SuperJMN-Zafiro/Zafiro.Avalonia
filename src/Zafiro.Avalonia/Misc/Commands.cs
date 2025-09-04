using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Services;

namespace Zafiro.Avalonia.ViewLocators;

public class Commands
{
    private readonly INotificationService notificationService;

    public Commands(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    public static Commands Instance { get; } = new(NotificationService.Instance);

    public ReactiveCommandBase<string, Result> LaunchUri => ReactiveCommand.CreateFromTask<string, Result>(str => Result.Try(() => LauncherService.Instance.LaunchUri(new Uri(str))));

    public ReactiveCommandBase<string, Result> CopyText => ReactiveCommand.CreateFromTask<string, Result>(async str =>
    {
        return await ApplicationUtils.GetClipboard()
            .ToResult("Cannot access clipboard")
            .Tap(clipboard => clipboard.SetTextAsync(str))
            .Tap(() => notificationService.Show(null!, "Copied to clipboard"));
    });
}