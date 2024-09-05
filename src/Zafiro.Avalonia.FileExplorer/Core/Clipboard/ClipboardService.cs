using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.Reactive;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace Zafiro.Avalonia.FileExplorer.Core.Clipboard;

public class ClipboardService : IClipboardService
{
    private const string MimeType = "x-special/zafiro-copied-files";

    public ClipboardService(IClipboard clipboard, ITransferManager transferManager,
        IEnumerable<IConnection> connections)
    {
        Clipboard = clipboard;
        TransferManager = transferManager;
        Connections = connections;
        CanPaste = Observable.Timer(TimeSpan.FromSeconds(0.5), AvaloniaScheduler.Instance)
            .Repeat()
            .Select(_ => GetCanPaste())
            .Concat()
            .ReplayLastActive()
            .ObserveOn(RxApp.MainThreadScheduler);
        
        CanPaste.Subscribe(b => { });
    }

    private IObservable<bool> GetCanPaste()
    {
        return Observable
            .FromAsync
            (
                () => GetCopiedItems().Map(clipboardEntries => clipboardEntries.Any())
            ).Successes();
    }

    public IObservable<bool> CanPaste { get; }

    public IClipboard Clipboard { get; }
    public ITransferManager TransferManager { get; }
    public IEnumerable<IConnection> Connections { get; }

    public async Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, FileSystemConnection connection)
    {
        var serialized = Serialize(items, sourcePath, connection);
        var dataObject = new DataObject();
        dataObject.Set(MimeType, serialized);
        await Clipboard.SetDataObjectAsync(dataObject);
        return Result.Success();
    }

    public Task<Result> Paste(IMutableDirectory destination)
    {
        var data = GetCopiedItems()
            .Bind(clipboardEntries => Paste(clipboardEntries, destination));

        return data;
    }

    private Task<Result<List<CopiedClipboardEntry>>> GetCopiedItems()
    {
        return Result.Try(() => Clipboard.GetDataAsync(MimeType))
            .EnsureNotNull("Nothing to copy")
            .Map(o => (byte[]?)o!)
            .Map(Decode)
            .Map(s => JsonSerializer.Deserialize<List<CopiedClipboardEntry>>(s)!);
    }

    private static string Decode(byte[] bytes)
    {
        if (OperatingSystem.IsLinux())
        {
            return Encoding.UTF8.GetString(bytes);
        }

        if (OperatingSystem.IsWindows())
        {
            return Encoding.Unicode.GetString(bytes).TrimEnd('\0');    
        }

        throw new NotSupportedException("Can't decode clipboards content");
    }

    private string Serialize(IEnumerable<IDirectoryItem> selectedItems, ZafiroPath parentPath, FileSystemConnection connection)
    {
        var toSerializationModel = ToSerializationModel(selectedItems, parentPath, connection);
        return JsonSerializer.Serialize(toSerializationModel);
    }

    private IEnumerable<CopiedClipboardEntry> ToSerializationModel(IEnumerable<IDirectoryItem> selectedItems,
        ZafiroPath parentPath, FileSystemConnection connection)
    {
        return selectedItems.Select(x => new CopiedClipboardEntry(x.Name, parentPath, connection.Identifier, x is FileViewModel ? ItemType.File : ItemType.Directory));
    }

    private async Task<Result> Paste(List<CopiedClipboardEntry> items, IMutableDirectory destination)
    {
        var transferItemResult = await GetAction(items, destination)
            .Map(action => (ITransferItem)new TransferItem($"Copiando {action.Actions.Count} elementos a {destination}", action));
        
        transferItemResult.Tap(transferItem =>
        {
            TransferManager.Add(transferItem);
            transferItem.Transfer.StartReactive.Execute().Subscribe();
        });
        
        return transferItemResult;
    }

    private Task<Result<CompositeAction>> GetAction(List<CopiedClipboardEntry> items, IMutableDirectory directory)
    {
        var results = items.Select(entry => ToCopyAction(entry, directory));
        var combine = results.CombineInOrder();
        return combine.Map(actions => new CompositeAction(actions.ToArray()));
    }

    private async Task<Result<IAction<LongProgress>>> ToCopyAction(CopiedClipboardEntry entry, IMutableDirectory directory)
    {
        var source = await FromEntry(entry);
        var destination = await directory.CreateFile(entry.Name);

        return source.CombineAndMap(destination, (src, dst) => (IAction<LongProgress>)new CopyFileAction(src, dst));
    }

    private Task<Result<IFile>> FromEntry(CopiedClipboardEntry entry)
    {
        var plugin = Connections.First(plugin => plugin.Identifier == entry.FileSystemKey);
        var folder = plugin.FileSystem.GetDirectory(entry.ParentPath);
        return folder
            .Map(x => x.GetFile(entry.Name))
            .Bind(x => x
                .Map(file => file.AsReadOnly()));
    }
}