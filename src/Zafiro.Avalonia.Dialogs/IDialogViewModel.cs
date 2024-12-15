namespace Zafiro.Avalonia.Dialogs;

public interface IDialogViewModel
{
    object ViewModel { get; }
    IEnumerable<IOption> Options { get; }
}