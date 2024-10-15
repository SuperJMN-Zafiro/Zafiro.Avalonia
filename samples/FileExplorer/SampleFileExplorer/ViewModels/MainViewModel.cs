using System.Collections.Generic;
using System.Linq;
using ClassLibrary1;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.Avalonia.FileExplorer.Tests;
using Zafiro.UI;

namespace SampleFileExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(List<FileSystemConnection> mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        Explorers = mutableFileSystem.Select(connection => new FileExplorer2(connection.FileSystem, directory => new DirectoryContents(directory)));
    }

    public IEnumerable<FileExplorer2> Explorers { get; }
}