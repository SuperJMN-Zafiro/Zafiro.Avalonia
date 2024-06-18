using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs.Simple;

public interface ISimpleDialog
{
    Task<bool> Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory, Maybe<Action<ConfigureSizeContext>> configureDialogAction);
}