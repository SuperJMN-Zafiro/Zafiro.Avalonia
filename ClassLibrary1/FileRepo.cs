using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Mixins;

namespace ClassLibrary1;

public class FileRepo : IFileRepo
{
    public IEnumerable<FileSystemConnection> Connections { get; }

    public FileRepo(IEnumerable<FileSystemConnection> connections)
    {
        Connections = connections;
    }
    
    public Task<Result<INode>> Get(FileLocator item)
    {
        return Locate(item);
    }

    private Task<Result<INode>> Locate(FileLocator item)
    {
        return Connections
            .TryFirst(x => x.Identifier == item.Connection)
            .ToResult(ErrorMessage(item))
            .Map(connection => GetItem(item, connection));
    }

    private string ErrorMessage(FileLocator item)
    {
        return $"Cannot find '{item.Connection}' in the available connections {{{Connections.Select(x => "'" + x.Identifier + "'").JoinWithCommas()}}}. Used: {item}";
    }

    private static Task<Result<INode>> GetItem(FileLocator item, FileSystemConnection connection)
    {
        return item.ItemType switch
        {
            ItemType.Invalid => throw new InvalidOperationException(),
            ItemType.File => connection.FileSystem.GetFile(item.Path).Map(INode (file) => file),
            ItemType.Directory => connection.FileSystem.GetDirectory(item.Path).Map(INode (file) => file),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}