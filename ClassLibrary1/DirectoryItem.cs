using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public class DirectoryItem(IRooted<IMutableDirectory> parent, string name, IFileExplorer fileExplorer) : IDirectoryItem
{
    public string Name { get; } = name;
    public string Key { get; } = name + "/";
    public ReactiveCommand<Unit, Result> Delete { get; } = ReactiveCommand.CreateFromTask(() => parent.Value.DeleteSubdirectory(name));
    public ReactiveCommand<Unit, Result<IDirectoryContents>> Navigate { get; } = ReactiveCommand.CreateFromTask(() => fileExplorer.GoTo(parent.Path.Combine(name)));
}