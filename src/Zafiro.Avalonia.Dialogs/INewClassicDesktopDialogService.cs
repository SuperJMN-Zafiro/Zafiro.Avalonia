﻿using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs;

public interface INewDialogService
{
    Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, params NewOptionConfiguration<TViewModel, TResult>[] options) where TViewModel : IHaveResult<TResult>;
}