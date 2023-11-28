using System.Collections.Generic;
using TestApp.Samples.Adorners;
using TestApp.Samples.MasterDetails;
using TestApp.Samples.StringEditor;
using TestApp.Samples.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialogService dialogService;
    private readonly IFilePicker storage;
    private readonly INotificationService notificationService;

    public MainViewModel(IDialogService dialogService, IFilePicker storage, INotificationService notificationService)
    {
        this.dialogService = dialogService;
        this.storage = storage;
        this.notificationService = notificationService;
    }

    public IEnumerable<Section> Sections => new List<Section>
    {
        new("MasterDetailsView", new MasterDetailsSampleViewModel()),
        new("Dialogs", new Samples.Dialogs.DialogSampleViewModel(notificationService, dialogService)),
        new("StringEditor", new StringEditorSampleViewModel()),
        new("Adorners", new AdornerSampleViewModel()),
        new("Wizard", new WizardSampleViewModel(dialogService, notificationService)),
        new("Storage", new StorageSampleViewModel(storage)),
        new("Behaviors", new BehaviorsSampleViewModel()),
    };
}