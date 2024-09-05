using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Navigator;
using Zafiro.Avalonia.FileExplorer.Core.Toolbar;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer;

public class FileExplorer : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> contents;

    public FileExplorer(FileSystemConnection connection, IMutableFileSystem mutableFileSystem, INotificationService notificationService, IDialog dialog,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        MutableFileSystem = mutableFileSystem;
        TransferManager = transferManager;
        PathNavigator = new PathNavigatorViewModel(mutableFileSystem, notificationService);

        var context = new ExplorerContext(PathNavigator, TransferManager, notificationService, dialog, clipboardService, connection);

        ToolBar = new ToolBarViewModel(context);
        contents = context.Directory.ToProperty(this, x => x.Contents);
        PathNavigator.SetAndLoad(mutableFileSystem.InitialPath);
    }

    public ToolBarViewModel ToolBar { get; }

    public DirectoryContentsViewModel Contents => contents.Value;
    public IMutableFileSystem MutableFileSystem { get; }
    public ITransferManager TransferManager { get; }
    public IPathNavigator PathNavigator { get; }
}