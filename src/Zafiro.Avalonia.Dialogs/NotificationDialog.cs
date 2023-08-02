using ReactiveUI;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

public class NotificationDialog : INotificationService
{
    private readonly IDialogService dialogService;

    public NotificationDialog(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public Task Show(string message)
    {
        var title = "Failure";
        var messageDialogViewModel = new MessageDialogViewModel(message);
        var dismissText = "OK";
        var optionConfiguration = new OptionConfiguration<MessageDialogViewModel>(dismissText, context => ReactiveCommand.Create(() => context.Window.Close()));
        return dialogService.ShowDialog(messageDialogViewModel, title, optionConfiguration);
    }
}