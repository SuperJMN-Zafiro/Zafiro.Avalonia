using System.Collections.Generic;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;
    private readonly IFilePicker storage;

    public MainViewModel(IDialogService dialogService, IFilePicker storage)
    {
        this.dialogService = dialogService;
        this.storage = storage;
    }

    public IEnumerable<Section> Sections => new List<Section>()
    {
        new("Dialogs", new DialogSampleViewModel(dialogService)),
        new("Storage", new StorageSampleViewModel(storage)),
    };
}