using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using ClassLibrary1;
using FluentAssertions;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.Tests;

public class ExplorerTests
{
    [Fact]
    public async Task Navigate_loads_contents()
    {
        var files = new Dictionary<string, MockFileData>()
        {
            ["TestDir"] = new MockDirectoryData(),
        };
        
        var sut = CreateSut(files);
        
        sut.Address = "TestDir";
        var result = await sut.LoadAddress.Execute();
    }

    [Fact]
    public async Task Navigate_to_path()
    {
        var files = new Dictionary<string, MockFileData>()
        {
            ["TestDir"] = new MockDirectoryData(),
        };
        var sut = CreateSut(files);
        
        sut.Address = "TestDir";
        var result = await sut.LoadAddress.Execute();
        
        result.Should().Succeed();
    }
    
    [Fact]
    public async Task Navigate_to_default()
    {
        var initialDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var files = new Dictionary<string, MockFileData>()
        {
            [initialDir] = new MockDirectoryData(),
        };
        var sut = CreateSut(files);
        
        var result = await sut.LoadAddress.Execute();
        
        result.Should().Succeed();
    }

    private static ClassLibrary1.FileExplorer CreateSut(Dictionary<string, MockFileData> files)
    {
        var mutableFileSystem = new MockFileSystem(files);
        var sut = new ClassLibrary1.FileExplorer(mutableFileSystem, (path, e) => new MockDirectoryContents(path));
        return sut;
    }
}

public class MockDirectoryContents(IRooted<IMutableDirectory> path) : IDirectoryContents
{
    public IEnumerable<IDirectoryItem> Items { get; }
    public IEnumerable<IDirectoryItem> SelectedItems { get; }
    public ZafiroPath Path { get; }
}