using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Simple;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task Show(this ISimpleDialog dialogService, object viewModel, string title, IObservable<bool> canSubmit)
    {
        return dialogService.Show(viewModel, title, closeable =>
        [
            new Option("Cancel", ReactiveCommand.Create(closeable.Close, canSubmit), false, true),
            new Option("OK", ReactiveCommand.Create(closeable.Close, Observable.Return(true)), true)
        ]);
    }
    
    public static Task ShowMessage(this ISimpleDialog dialogService, string title, string text, string dismissText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text, DialogSizeCalculator.CalculateDialogWidth(text));
        
        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            new Option(dismissText, ReactiveCommand.Create(closeable.Close, Observable.Return(true)), true)
        ]);
    }
}