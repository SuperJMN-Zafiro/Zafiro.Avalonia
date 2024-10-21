using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public partial class FileItem(IRooted<IMutableDirectory> parent, IMutableFile file) : ReactiveObject, IDirectoryItemFile
{
    [Reactive] private bool isSelected;
    public string Name { get; } = file.Name;
    public string Key { get; } = file.GetKey();
    public ReactiveCommand<Unit, Result> Delete { get; } = ReactiveCommand.CreateFromTask(() => parent.Value.DeleteFile(file.Name));
}