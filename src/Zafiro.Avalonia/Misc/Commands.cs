using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Services;

namespace Zafiro.Avalonia.Misc;

public class Commands
{
    private readonly INotificationService notificationService;

    public Commands(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    public static Commands Instance { get; } = new(NotificationService.Instance);

    public ReactiveCommandBase<Uri, Result> LaunchUri => ReactiveCommand.CreateFromTask<Uri, Result>(str => Result.Try(() => LauncherService.Instance.LaunchUri(str)));

    public ReactiveCommandBase<string, Result> CopyText => ReactiveCommand.CreateFromTask<string, Result>(async str =>
    {
        return await ApplicationUtils.GetClipboard()
            .ToResult("Cannot access clipboard")
            .Tap(clipboard => clipboard.SetTextAsync(str))
            .Tap(() => notificationService.Show(null!, "Copied to clipboard"));
    });
}