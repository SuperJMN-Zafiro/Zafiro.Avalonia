using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public class DirectoryItem(IMutableDirectory parent, IMutableDirectory directory) : IDirectoryItem
{
    public string Name { get; } = directory.Name;
    public string Key { get; } = directory.GetKey();
    public ReactiveCommand<Unit, Result> Delete { get; } = DeleteCommand(parent, directory);

    private static ReactiveCommand<Unit, Result> DeleteCommand(IMutableDirectory parent, IMutableDirectory directory)
    {
        return ReactiveCommand.CreateFromTask(() => parent.DeleteSubdirectory(directory.Name));
    }
}