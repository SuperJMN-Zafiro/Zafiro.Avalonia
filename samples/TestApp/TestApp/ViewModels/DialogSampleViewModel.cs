using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;

namespace TestApp.ViewModels;

public class DialogSampleViewModel
{
    public DialogSampleViewModel(IDialogService dialogService)
    {
        ShowMessageCommand = ReactiveCommand.CreateFromTask(() => OnShowMessage(dialogService));
    }

    public ReactiveCommand<Unit, Unit> ShowMessageCommand { get; }

    private static Task OnShowMessage(IDialogService dialogService)
    {
        var ok = new OptionConfiguration<MessageViewModel>("Close", x => ReactiveCommand.Create(() => x.Closeable.Close()));
        return dialogService.ShowDialog(new MessageViewModel() { Text = "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate"}, "Dialog Title", ok);
    }
}