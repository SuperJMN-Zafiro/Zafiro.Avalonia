﻿using System.Diagnostics.CodeAnalysis;
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
            OptionBuilder.Create("Cancel", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss, Observable.Return(true))), new Settings
            {
                IsDefault = false,
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("OK", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, canSubmit)), new Settings()
            {
                IsDefault = true,
            })
        ]);
    }
    
    public static async Task<Maybe<TResult>> ShowAndGetResult<TViewModel, TResult>(this IDialog dialogService,
        [DisallowNull] TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
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
        [DisallowNull] TViewModel viewModel, string title, Func<TViewModel, IObservable<bool>> canSubmit,
        Func<TViewModel, TResult> getResult)
    {
        Func<ICloseable, IOption[]> optionsFactory = closeable =>
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
        ];
        
        return await dialogService.ShowAndGetResult(viewModel, title, canSubmit, optionsFactory, getResult);
    }
    
    public static async Task<Maybe<bool>> ShowConfirmation(this IDialog dialogService, string title, string text, string yesText = "Yes", string noText = "No")
    {
        var result = false;
        
        var show = await dialogService.Show(new MessageDialogViewModel(text), title, closeable =>
        {
            List<IOption> options =
            [
                OptionBuilder.Create(yesText, EnhancedCommand.Create(ReactiveCommand.Create(() =>
                {
                    result = true;
                    closeable.Close();
                })), new Settings()),
                OptionBuilder.Create(noText, EnhancedCommand.Create(ReactiveCommand.Create(() =>
                {
                    result = false;
                    closeable.Close();
                })), new Settings())
            ];
            
            return options;
        });

        if (show)
        {
            return result;
        }

        return Maybe<bool>.None;
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