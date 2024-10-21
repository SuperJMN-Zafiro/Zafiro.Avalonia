using System.Reactive;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public interface IFileExplorer
{
    IObservable<IChangeSet<IDirectoryItem, string>> Items { get; }
    string Key { get; }
    ReactiveCommand<Unit, Result<IDirectoryContents>> GoBack { get; }
    IObservable<IDirectoryContents> Contents { get; }
    ReactiveCommand<Unit, Result<IDirectoryContents>> LoadAddress { get; }
    string Address { get; }
    public ReactiveCommand<Unit, Unit> Copy { get; }
    Task<Result<IDirectoryContents>> GoTo(ZafiroPath path);
}