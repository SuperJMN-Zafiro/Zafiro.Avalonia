using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Avalonia.Controls.Selection;
using ClassLibrary1;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Primitives;
using Zafiro.Avalonia.FileExplorer.Core;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.FileExplorer.Tests;

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