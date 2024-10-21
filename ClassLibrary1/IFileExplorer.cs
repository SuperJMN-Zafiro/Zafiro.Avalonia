using System.Collections.ObjectModel;
using System.Reactive;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public interface IFileExplorer
{
    Task<Result<IDirectoryContents>> GoTo(ZafiroPath newAddress);
    IObservable<IChangeSet<IDirectoryItem, string>> Items { get; }
}