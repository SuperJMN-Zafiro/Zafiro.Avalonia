using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
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
        new("Dialogs", new Samples.Dialogs.DialogSampleViewModel(notificationService, dialogService)),
        // TODO: Add wizard sample
        new("Wizard", new WizardSampleViewModel(dialogService, notificationService)),
        new("Storage", new StorageSampleViewModel(storage)),
        new("Behaviors", new BehaviorsSampleViewModel()),
    };
}