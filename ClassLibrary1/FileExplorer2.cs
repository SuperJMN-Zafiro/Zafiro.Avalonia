using System.Reactive;
using ClassLibrary1;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.Tests;

public partial class FileExplorer2 : ReactiveObject
{
    [Reactive] private string address;

    public FileExplorer2(IMutableFileSystem fileSystem, Func<IMutableDirectory, IDirectoryContents> getContents)
    {
        Navigate = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(Address).Map(getContents));
        Address = fileSystem.InitialPath;
        Contents = Navigate.Successes();
    }

    public IObservable<IDirectoryContents> Contents { get; }

    public ReactiveCommand<Unit, Result<IDirectoryContents>> Navigate { get; }
}