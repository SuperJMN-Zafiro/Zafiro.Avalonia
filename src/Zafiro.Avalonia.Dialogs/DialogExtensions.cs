using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task<bool> Show(this IDialog dialogService, object viewModel, string title,
        Func<ICloseable, IOption[]> optionsFactory)
    {
        return dialogService.Show(viewModel, title, optionsFactory);
    }

    public static Task Show(this IDialog dialogService, object viewModel, string title,
        IObservable<bool> canSubmit)
    {
        return dialogService.Show(viewModel, title, closeable =>
        [
            OptionBuilder.Create("Cancel", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, canSubmit)), false, true),
            OptionBuilder.Create("OK", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, Observable.Return(true))), true)
        ]);
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult)
    {
        var dialogResult = await dialogService.Show(viewModel, title, dialog =>
        [
            OptionBuilder.Create("Cancel", EnhancedCommand.Create(ReactiveCommand.Create(dialog.Dismiss, Observable.Return(true))), false, true),
            OptionBuilder.Create("OK", EnhancedCommand.Create(ReactiveCommand.Create(dialog.Close, canSubmit(viewModel))), true)
        ]);

        if (dialogResult == false) return Maybe<TResult>.None;

        return getResult(viewModel);
    }
    
    public static Task<bool> ShowConfirmation(this IDialog dialogService, string title, string text)
    {
        var messageDialogViewModel = new MessageDialogViewModel(text);

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            OptionBuilder.Create("Yes", EnhancedCommand.Create(ReactiveCommand.Create(() => closeable.Close()))),
            OptionBuilder.Create("No", EnhancedCommand.Create(ReactiveCommand.Create(() => closeable.Dismiss())))
        ]);
    }

    public static Task ShowMessage(this IDialog dialogService, string title, string text,
        string dismissText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text);

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            OptionBuilder.Create(dismissText, EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, Observable.Return(true))), true)
        ]);
    }
}