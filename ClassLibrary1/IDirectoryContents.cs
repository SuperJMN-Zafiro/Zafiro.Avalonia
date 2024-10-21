using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public interface IDirectoryContents
{
    public IEnumerable<IDirectoryItem> Items { get; }
    ZafiroPath Path { get; }
}