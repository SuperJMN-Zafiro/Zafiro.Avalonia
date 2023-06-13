using System.Collections.Generic;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Interfaces;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;
    private readonly IStorage storage;

    public MainViewModel(IDialogService dialogService, IStorage storage)
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