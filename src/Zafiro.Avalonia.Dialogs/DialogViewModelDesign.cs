using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs;

public class DialogViewModelDesign : IDialogViewModel
{
    public object ViewModel { get; }
    public IEnumerable<Option> Options { get; }

    public DialogViewModelDesign()
    {
        ViewModel = new object();
        var doNothing = ReactiveCommand.Create(() => {});
        Options = new Option[]{ new("OK", doNothing), new("Cancel", doNothing)};
    }
}