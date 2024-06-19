using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Simple;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task<bool> Show(this IDialog dialogService, object viewModel, string title,
        Func<ICloseable, Option[]> optionsFactory)
    {
        return dialogService.Show(viewModel, title, optionsFactory);
    }

    public static Task Show(this IDialog dialogService, object viewModel, string title,
        IObservable<bool> canSubmit, Maybe<Action<ConfigureSizeContext>> configureDialogAction)
    {
        return dialogService.Show(viewModel, title, closeable =>
        [
            new Option("Cancel", ReactiveCommand.Create(closeable.Close, canSubmit), false, true),
            new Option("OK", ReactiveCommand.Create(closeable.Close, Observable.Return(true)), true)
        ]);
    }

    public static Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult)
    {
        return ShowAndGetResult(dialogService, viewModel, title, canSubmit, getResult,
            Maybe<Action<ConfigureSizeContext>>.None);
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult, Maybe<Action<ConfigureSizeContext>> configureDialogAction)
    {
        var dialogResult = await dialogService.Show(viewModel, title, dialog =>
        [
            new Option("Cancel", ReactiveCommand.Create(dialog.Dismiss, Observable.Return(true)), false, true),
            new Option("OK", ReactiveCommand.Create(dialog.Close, canSubmit(viewModel)), true)
        ]);

        if (dialogResult == false) return Maybe<TResult>.None;

        return getResult(viewModel);
    }
    
    public static Task<bool> ShowConfirmation(this IDialog dialogService, string title, string text)
    {
        var messageDialogViewModel = new MessageDialogViewModel(text, DialogSizeCalculator.CalculateDialogWidth(text));

        return dialogService.Show(messageDialogViewModel, title, closeable => new []
        {
            new Option("Yes", ReactiveCommand.Create(() => closeable.Close())),
            new Option("No", ReactiveCommand.Create(() => closeable.Dismiss()))
        });
    }

    public static Task ShowMessage(this IDialog dialogService, string title, string text,
        string dismissText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text, DialogSizeCalculator.CalculateDialogWidth(text));

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            new Option(dismissText, ReactiveCommand.Create(closeable.Close, Observable.Return(true)), true)
        ]);
    }

    private static void ConfigureForMessage(ConfigureSizeContext context, string text)
    {
        context.Height = double.NaN;
        context.Width = DialogSizeCalculator.CalculateDialogWidth(text);
    }
}