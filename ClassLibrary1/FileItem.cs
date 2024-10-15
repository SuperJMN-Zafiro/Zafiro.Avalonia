using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public class FileItem(IMutableDirectory parent, IMutableFile file) : IDirectoryItem
{
    public string Name { get; } = file.Name;
    public string Key { get; } = file.GetKey();
    public ReactiveCommand<Unit, Result> Delete { get; } = DeleteCommand(parent, file);

    private static ReactiveCommand<Unit, Result> DeleteCommand(IMutableDirectory mutableDirectory, IMutableFile file)
    {
        return ReactiveCommand.CreateFromTask(() => mutableDirectory.DeleteFile(file.Name));
    }
}