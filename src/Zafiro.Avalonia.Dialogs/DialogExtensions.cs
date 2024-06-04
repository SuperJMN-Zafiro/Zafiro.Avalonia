using System.Reactive.Linq;
using Zafiro.Avalonia.Dialogs.Simple;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task ShowMessage(this ISimpleDialog dialogService, string title, string text, string dismissText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text, DialogSizeCalculator.CalculateDialogWidth(text));
        return dialogService.Show(messageDialogViewModel, title, Observable.Return(true));
    }
}