namespace Zafiro.Avalonia.Dialogs;

public class DialogViewModel
{
    public object ViewModel { get; }
    public IEnumerable<IOption> Options { get; }

    public DialogViewModel(object viewModel, params IOption[] options)
    {
        ViewModel = viewModel;
        Options = options;
    }
}