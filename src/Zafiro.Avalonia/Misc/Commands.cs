using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Services;

namespace Zafiro.Avalonia.Misc;

public class Commands
{
    private readonly INotificationService notificationService;
    
    public static Commands Instance { get; } = new(NotificationService.Instance);

    public Commands(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }
    
    public ReactiveCommandBase<string, Result> LaunchUri => ReactiveCommand.CreateFromTask<string, Result>(str => Result.Try(() => LauncherService.Instance.LaunchUri(new Uri(str))));
    public ReactiveCommandBase<string, Result> CopyText => ReactiveCommand.CreateFromTask<string, Result>(async str =>
    {
        return await Application.Current!.TopLevel()
            .Map(x => x.Clipboard)
            .EnsureNotNull("Clipboard is null!")
            .Tap(clipboard => clipboard.SetTextAsync(str))
            .Tap(() => notificationService.Show(null!, "Copied to clipboard"));
    });
    
}