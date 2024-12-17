namespace Zafiro.Avalonia.Dialogs;

public interface IDialog
{
    Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory);
}