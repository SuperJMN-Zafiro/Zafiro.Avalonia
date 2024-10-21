using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ClassLibrary1;
using DynamicData;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.Avalonia.FileExplorer.Tests;
using Zafiro.UI;
using FileExplorer = ClassLibrary1.FileExplorer;

namespace SampleFileExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(List<FileSystemConnection> mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        var fileExplorers = mutableFileSystem
            .Select(connection => new FileExplorer(connection.FileSystem, (directory, e) => new DirectoryContents(directory, e))).ToList();
        Explorers = fileExplorers;

        fileExplorers[0].Items.AutoRefresh(x => x.IsSelected).Filter(x => x.IsSelected).Subscribe(set => { });
    }

    public IEnumerable<FileExplorer> Explorers { get; }
}