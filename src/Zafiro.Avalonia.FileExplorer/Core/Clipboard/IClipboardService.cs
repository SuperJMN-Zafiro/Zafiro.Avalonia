using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

namespace Zafiro.Avalonia.FileExplorer.Core.Clipboard;

public interface IClipboardService
{
    public Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, FileSystemConnection connection);
    public Task<Result> Paste(IMutableDirectory destination);
    IObservable<bool> CanPaste { get; }
}