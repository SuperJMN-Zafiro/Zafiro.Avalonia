using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Dialogs.Views;

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
            OptionBuilder.Create("Cancel", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss, canSubmit)), new Settings
            {
                IsDefault = false,
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("OK", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, Observable.Return(true))), new Settings()
            {
                IsDefault = true,
            })
        ]);
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult)
    {
        var dialogResult = await dialogService.Show(viewModel, title, closeable =>
        [
            OptionBuilder.Create("Cancel", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss)), new Settings
            {
                IsDefault = false,
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("OK", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, canSubmit(viewModel))), new Settings()
            {
                IsDefault = true,
            })
        ]);

        if (dialogResult == false) return Maybe<TResult>.None;

        return getResult(viewModel);
    }
    
    public static Task<bool> ShowConfirmation(this IDialog dialogService, string title, string text)
    {
        var messageDialogViewModel = new MessageDialogViewModel(text);

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            OptionBuilder.Create("Yes", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close)), new Settings()),
            OptionBuilder.Create("No", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close)), new Settings())
        ]);
    }

    public static Task ShowMessage(this IDialog dialogService, string title, string text,
        string okText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text);

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            OptionBuilder.Create(okText, EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, Observable.Return(true))), new Settings()
            {
                IsDefault = true
            })
        ]);
    }
}