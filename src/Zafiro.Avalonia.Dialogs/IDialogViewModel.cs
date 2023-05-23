namespace Zafiro.Avalonia.Dialogs;

public interface IDialogViewModel
{
    object ViewModel { get; }
    IEnumerable<Option> Options { get; }
}