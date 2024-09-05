using System.Collections.Generic;
using System.Linq;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.UI;

namespace SampleFileExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(List<FileSystemConnection> mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        Explorers = mutableFileSystem.Select(connection => new FileExplorer(connection, connection.FileSystem, notificationService, dialogService, clipboardService, transferManager));
    }

    public IEnumerable<FileExplorer> Explorers { get; }
}