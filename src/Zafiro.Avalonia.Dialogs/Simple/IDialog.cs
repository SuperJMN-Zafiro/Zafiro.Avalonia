namespace Zafiro.Avalonia.Dialogs.Simple;

public interface IDialog
{
    Task<bool> Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory);
}