using System.Reactive;
using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs;

public class DialogViewModel : IDialogViewModel
{
    public object ViewModel { get; }
    public IEnumerable<Option> Options { get; }

    public DialogViewModel(object viewModel, params Option[] options)
    {
        ViewModel = viewModel;
        Options = options;
    }

    public ReactiveCommand<Unit, Unit> Close { get; set; }
}