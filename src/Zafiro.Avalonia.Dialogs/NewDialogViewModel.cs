namespace Zafiro.Avalonia.Dialogs;

public class NewDialogViewModel : IDialogViewModel
{
    public object ViewModel { get; }
    public IEnumerable<Option> Options { get; }
    public string Title { get; set; }

    public NewDialogViewModel(object viewModel, string title = "", params Option[] options)
    {
        ViewModel = viewModel;
        Title = title;
        Options = options;
    }
}