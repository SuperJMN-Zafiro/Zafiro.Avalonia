using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogExtensions
{
    public static Task<bool> Show(this IDialog dialogService,
        object viewModel,
        string title,
        Func<ICloseable, IOption[]> optionsFactory)
    {
        return dialogService.Show(viewModel, title, optionsFactory);
    }

    public static Task ShowOk(this IDialog dialogService,
        object viewModel,
        string title,
        IObservable<bool>? canSubmit = null)
    {
        return dialogService.Show(viewModel, title, closeable =>
        [
            OptionBuilder.Create("OK", ReactiveCommand.Create(closeable.Close, canSubmit).Enhance(), new Settings()
            {
                IsDefault = true,
            })
        ]);
    }

    public static Task Show(this IDialog dialogService,
        object viewModel,
        string title,
        IObservable<bool>? canSubmit)
    {
        return dialogService.Show(viewModel, title, closeable =>
        [
            OptionBuilder.Create("Cancel", ReactiveCommand.Create(closeable.Dismiss, Observable.Return(true)).Enhance(), new Settings
            {
                IsDefault = false,
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("OK", ReactiveCommand.Create(closeable.Close, canSubmit).Enhance(), new Settings()
            {
                IsDefault = true,
            })
        ]);
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel,
        string title,
        Func<ICloseable, IEnumerable<IOption>> optionsFactory,
        Func<TViewModel, TResult> getResult)
    {
        bool showed = await dialogService.Show(viewModel, title, optionsFactory);

        if (showed)
        {
            return getResult(viewModel);
        }

        return Maybe<TResult>.None;
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel,
        string title,
        Func<ICloseable, IEnumerable<IOption>> optionsFactory,
        Func<TViewModel, Task<TResult>> getResult)
    {
        bool showed = await dialogService.Show(viewModel, title, optionsFactory);

        if (showed)
        {
            return await getResult(viewModel);
        }

        return Maybe<TResult>.None;
    }

    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel,
        string title,
        Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult)
    {
        Func<ICloseable, IOption[]> optionsFactory = closeable =>
        [
            OptionBuilder.Create("Cancel", ReactiveCommand.Create(closeable.Dismiss).Enhance(), new Settings
            {
                IsDefault = false,
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("OK", ReactiveCommand.Create(closeable.Close, canSubmit(viewModel)).Enhance(), new Settings()
            {
                IsDefault = true,
            })
        ];

        return await dialogService.ShowAndGetResult(viewModel, title, optionsFactory, getResult);
    }

    public static async Task<Maybe<bool>> ShowConfirmation(this IDialog dialogService, string title, string text, string yesText = "Yes", string noText = "No")
    {
        var result = false;

        var show = await dialogService.Show(new MessageDialogViewModel(text), title, closeable =>
        {
            List<IOption> options =
            [
                OptionBuilder.Create(yesText, ReactiveCommand.Create(() =>
                {
                    result = true;
                    closeable.Close();
                }).Enhance(), new Settings()),
                OptionBuilder.Create(noText, ReactiveCommand.Create(() =>
                {
                    result = false;
                    closeable.Close();
                }).Enhance(), new Settings())
            ];

            return options;
        });

        if (show)
        {
            return result;
        }

        return Maybe<bool>.None;
    }

    public static Task ShowMessage(this IDialog dialogService,
        string title,
        string text,
        string okText = "OK")
    {
        var messageDialogViewModel = new MessageDialogViewModel(text);

        return dialogService.Show(messageDialogViewModel, title, closeable =>
        [
            OptionBuilder.Create(okText, ReactiveCommand.Create(closeable.Close, Observable.Return(true)).Enhance(), new Settings()
            {
                IsDefault = true
            })
        ]);
    }
}