using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.Adorners;
using TestApp.Samples.Controls;
using TestApp.Samples.ControlsNew;
using TestApp.Samples.DataAnalysis;
using TestApp.Samples.Diagrams;
using TestApp.Samples.Drag;
using TestApp.Samples.MasterDetails;
using TestApp.Samples.Panels;
using TestApp.Samples.StringEditor;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDialog dialogService;
    private readonly INotificationService notificationService;
    private readonly IFileSystemPicker storage;

    public MainViewModel(IDialog dialogService, IFileSystemPicker storage, INotificationService notificationService)
    {
        this.dialogService = dialogService;
        this.storage = storage;
        this.notificationService = notificationService;
    }

    public IEnumerable<SectionOld> Sections => new List<SectionOld>
    {
        new("MasterDetailsView", new MasterDetailsSampleViewModel(), Maybe<object>.None),
        new("Dialogs", new Samples.Dialogs.DialogSampleViewModel(notificationService, dialogService), Maybe<object>.None),
        new("Controls", new ControlsSampleViewModel(), Maybe<object>.None),
        new("More controls", new ControlsViewModel(), Maybe<object>.None),
        new("Panel", new PanelsSectionViewModel(), Maybe<object>.None),
        new("StringEditor", new StringEditorSampleViewModel(), Maybe<object>.None),
        new("Adorners", new AdornerSampleViewModel(), Maybe<object>.None),
        new("Storage", new StorageSampleViewModel(storage), Maybe<object>.None),
        new("Behaviors", new BehaviorsSampleViewModel(), Maybe<object>.None),
        new("Diagrams", new DiagramsViewModel(), Maybe<object>.None),
        new("Data Analysis", new DataAnalysisViewModel(), Maybe<object>.None),
        new("Drag", new DragViewModel(), Maybe<object>.None)
    };
}