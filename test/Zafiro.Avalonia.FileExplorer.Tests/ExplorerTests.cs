using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using FluentAssertions;

namespace Zafiro.Avalonia.FileExplorer.Tests;

public class ExplorerTests
{
    [Fact]
    public async Task Navigate_to_path()
    {
        var files = new Dictionary<string, MockFileData>()
        {
            ["TestDir"] = new MockDirectoryData(),
        };
        var sut = new FileExplorer2(new MockFileSystem(files));
        
        sut.Address = "TestDir";
        var result = await sut.Navigate.Execute();
        
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
        
        var mutableFileSystem = new MockFileSystem(files);
        var sut = new FileExplorer2(mutableFileSystem);
        
        var result = await sut.Navigate.Execute();
        
        result.Should().Succeed();
    }
}