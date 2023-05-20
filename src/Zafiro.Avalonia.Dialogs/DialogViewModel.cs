using System.Collections;

namespace Zafiro.UI.Avalonia;

public class DialogViewModel : IDialogViewModel
{
    public object ViewModel { get; }
    public IEnumerable<Option> Options { get; }
    public string Title { get; set; }

    public DialogViewModel(object viewModel, string title = "", params Option[] options)
    {
        ViewModel = viewModel;
        Title = title;
        Options = options;
    }
}