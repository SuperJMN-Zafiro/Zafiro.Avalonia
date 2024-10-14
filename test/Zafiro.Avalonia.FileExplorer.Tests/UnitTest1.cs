using System.IO.Abstractions.TestingHelpers;
using System.Reactive;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Primitives;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.FileExplorer.Tests;

public partial class FileExplorer2 : ReactiveObject
{
    [Reactive] private ZafiroPath address;

    public FileExplorer2(IMutableFileSystem fileSystem)
    {
        Navigate = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(Address));
        Address = fileSystem.InitialPath;
    }

    public ReactiveCommand<Unit, Result<IMutableDirectory>> Navigate { get; }
}

public class UnitTest1
{
    [Fact]
    public async Task Get_non_existing_filesystem()
    {
        var fileUniverse = new FileRepo([new FileSystemConnection("test", "Test", new MockFileSystem())]);
        var file = await fileUniverse.Get(new FileLocator("local", "home/jmn/file.txt", ItemType.File));
        file.Should().Fail();
    }
    
    [Fact]
    public async Task Get_non_existing_file()
    {
        var fileUniverse = new FileRepo([new FileSystemConnection("test", "Test", new MockFileSystem())]);
        var file = await fileUniverse.Get(new FileLocator("test", "home/jmn/file.txt", ItemType.File));
        file.Should().Fail();
    }
    
    [Fact]
    public async Task Get_existing()
    {
        var filesystem = new Dictionary<string, MockFileData>()
        {
            ["home/jmn/file.txt"] = new(""),
        };
        
        var fileUniverse = new FileRepo([new FileSystemConnection("test", "Test", new MockFileSystem(filesystem))]);
        var file = await fileUniverse.Get(new FileLocator("test", "home/jmn/file.txt", ItemType.File));
        file.Should().Succeed();
    }
}

public interface IFileRepo 
{
    public Task<Result<INode>> Get(FileLocator item);
}

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

public static class StringAssertionExtensions
{
    public static AndConstraint<StringAssertions> BeIgnoringWhitespace(this StringAssertions assertions, string expected)
    {
        string actualNormalized = Regex.Replace(assertions.Subject, @"\s+", "");
        string expectedNormalized = Regex.Replace(expected, @"\s+", "");
        return actualNormalized.Should().Be(expectedNormalized);
    }
}



public class MockFileSystem : IMutableFileSystem
{
    public System.IO.Abstractions.TestingHelpers.MockFileSystem Inner { get; }

    public Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path)
    {
        return FileSystem.GetDirectory(path);
    }

    public MockFileSystem(IDictionary<string,MockFileData>? files = null)
    {
        Inner = new System.IO.Abstractions.TestingHelpers.MockFileSystem(files);
        FileSystem = new Zafiro.FileSystem.Local.FileSystem(Inner);
    }

    public IMutableFileSystem FileSystem { get; }

    public ZafiroPath InitialPath => FileSystem.InitialPath;
}