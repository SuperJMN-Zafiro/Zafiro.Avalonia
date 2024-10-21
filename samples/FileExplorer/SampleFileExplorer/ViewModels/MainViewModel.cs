using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClassLibrary1;
using DynamicData;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.UI;
using FileExplorer = ClassLibrary1.FileExplorer;

namespace SampleFileExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(List<FileSystemConnection> mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        
        var fileContext = new FileContext();

        foreach (var connection in mutableFileSystem)
        {
            fileContext.Add(new FileExplorer(connection, (directory, e) => new DirectoryContents(directory, e), new Clipboard()));
        }
        
        Explorers = fileContext.Explorers;
    }

    public ReadOnlyObservableCollection<IFileExplorer> Explorers { get; }
}