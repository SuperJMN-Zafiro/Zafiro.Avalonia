using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

namespace ClassLibrary1;

public interface IDirectoryContents
{
    public IEnumerable<IDirectoryItem> Items { get; }
    public IEnumerable<IDirectoryItem> SelectedItems { get;  }
}