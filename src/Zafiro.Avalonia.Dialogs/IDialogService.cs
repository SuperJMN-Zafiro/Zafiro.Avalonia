using CSharpFunctionalExtensions;
using Zafiro.Avalonia.MigrateToZafiro;

namespace Zafiro.Avalonia.Dialogs;

public interface IDialogService
{
    Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, params OptionConfiguration<TViewModel, TResult>[] options) where TViewModel : UI.IResult<TResult>;
}