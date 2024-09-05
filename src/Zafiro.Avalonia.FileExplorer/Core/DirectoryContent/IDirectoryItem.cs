namespace Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

public interface IDirectoryItem : INamed
{
    public string Key { get;  }
    IObservable<Unit> Deleted { get; }
    ReactiveCommand<Unit, Result> Delete { get; }
}