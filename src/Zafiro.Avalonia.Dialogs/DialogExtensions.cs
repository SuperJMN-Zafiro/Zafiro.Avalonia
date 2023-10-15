using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(this IDialogService dialog, TViewModel viewModel, string title) where TViewModel : UI.IResult<TResult>
    {
        return dialog.ShowDialog(viewModel, title, model => Observable.FromAsync(() => model.Result), Array.Empty<OptionConfiguration<TViewModel, TResult>>());
    }

    public static Task ShowMessage(this IDialogService dialogService, string dismissText, string title, string text)
    {
        var optionConfiguration = new OptionConfiguration<MessageDialogViewModel, Unit>(dismissText, model => ReactiveCommand.Create(() => model.SetResult(Unit.Default)));
        var messageDialogViewModel = new MessageDialogViewModel(text);
        return dialogService.ShowDialog(messageDialogViewModel, title, model => Observable.FromAsync(() => model.Result), optionConfiguration);
    }
}