using System.Reactive;
using System.Reactive.Linq;
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

    private static async Task OnShowMessage(IDialogService dialogService)
    {
        await dialogService.ShowMessage("Dismiss", "Dialog Title", "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate");
    }
}