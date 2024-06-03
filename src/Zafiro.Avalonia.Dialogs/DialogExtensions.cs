using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(this IDialogService dialog, TViewModel viewModel, string title, Maybe<Action<ConfigureWindowContext>> configureWindowActionOverride) where TViewModel : UI.IResult<TResult>
    {
        return dialog.ShowDialog(viewModel, title, model => Observable.FromAsync(() => model.Result), configureWindowActionOverride, Array.Empty<OptionConfiguration<TViewModel, TResult>>());
    }

    public static Task ShowMessage(this IDialogService dialogService, string title, string text, string dismissText = "OK")
    {
        var optionConfiguration = new OptionConfiguration<MessageDialogViewModel, Unit>(dismissText, model => ReactiveCommand.Create(() => model.SetResult(Unit.Default)));
        var messageDialogViewModel = new MessageDialogViewModel(text, DialogSizeCalculator.CalculateDialogWidth(text));
        var doNothingWithWindow = Maybe<Action<ConfigureWindowContext>>.From(_ => { });
        return dialogService.ShowDialog(messageDialogViewModel, title, model => Observable.FromAsync(() => model.Result), doNothingWithWindow, optionConfiguration);
    }
}