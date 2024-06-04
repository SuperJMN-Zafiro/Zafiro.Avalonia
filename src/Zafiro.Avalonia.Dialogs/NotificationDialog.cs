using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

[PublicAPI]
public class NotificationDialog : INotificationService
{
    private readonly ISimpleDialog dialogService;

    public NotificationDialog(ISimpleDialog dialogService)
    {
        this.dialogService = dialogService;
    }

    public Task Show(string message, Maybe<string> title)
    {
        return dialogService.ShowMessage(title.GetValueOrDefault(), message, DismissText.GetValueOrDefault("Dismiss"));
    }

    public Maybe<string> DismissText { get; set; }
}