
namespace Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

public interface IDirectoryItem : INamed, ISelectable, IReactiveObject
{
    public string Key { get;  }
    ReactiveCommand<Unit, Result> Delete { get; }
}

public interface IDirectoryItemFile : IDirectoryItem;

public interface IDirectoryItemDirectory : IDirectoryItem;