using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Obsolete;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

[PublicAPI]
public class NotificationDialog : INotificationService
{
    private readonly Obsolete.IDialogService dialogService;

    public NotificationDialog(Obsolete.IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public Task Show(string message, Maybe<string> title)
    {
        var messageDialogViewModel = new MessageDialogViewModel(message);
        var dismissText = "OK";
        var optionConfiguration = new OptionConfiguration<MessageDialogViewModel>(dismissText, context => ReactiveCommand.Create(() => context.Closeable.Close()));
        return dialogService.ShowDialog(messageDialogViewModel, title.GetValueOrDefault("Failure"), optionConfiguration);
    }
}