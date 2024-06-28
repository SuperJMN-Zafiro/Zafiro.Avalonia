﻿using System.Collections.Generic;
using TestApp.Samples.Adorners;
using TestApp.Samples.Controls;
using TestApp.Samples.MasterDetails;
using TestApp.Samples.StringEditor;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialog dialogService;
    private readonly IFileSystemPicker storage;
    private readonly INotificationService notificationService;

    public MainViewModel(IDialog dialogService, IFileSystemPicker storage, INotificationService notificationService)
    {
        this.dialogService = dialogService;
        this.storage = storage;
        this.notificationService = notificationService;
    }

    public IEnumerable<Section> Sections => new List<Section>
    {
        new("MasterDetailsView", new MasterDetailsSampleViewModel()),
        new("Dialogs", new Samples.Dialogs.DialogSampleViewModel(notificationService, dialogService)),
        new("Controls", new ControlsSampleViewModel()),
        new("StringEditor", new StringEditorSampleViewModel()),
        new("Adorners", new AdornerSampleViewModel()),
        new("Storage", new StorageSampleViewModel(storage)),
        new("Behaviors", new BehaviorsSampleViewModel()),
    };
}