namespace Zafiro.UI.Avalonia;

public interface IDialogViewModel
{
    object ViewModel { get; }
    IEnumerable<Option> Options { get; }
}