﻿using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;

namespace TestApp.ViewModels;

public class DialogSampleViewModel
{
    public DialogSampleViewModel(IDialog dialogService)
    {
        ShowMessageCommand = ReactiveCommand.CreateFromTask(() => OnShowMessage(dialogService));
    }

    public ReactiveCommand<Unit, Unit> ShowMessageCommand { get; }

    private static async Task OnShowMessage(IDialog dialogService)
    {
        await dialogService.ShowMessage("Dialog Title", "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate", "Dismiss");
    }
}