using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable.Mutable;
using Zafiro.FileSystem.Readonly;
using Directory = Zafiro.FileSystem.Readonly.Directory;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.Avalonia.Storage;

public static class MutableMixin
{
    public static Task<Result<IDirectory>> ToImmutable(this IMutableDirectory directory)
    {
        return directory
            .MutableChildren()
            .BindAndCombine(ToImmutable)
            .Map(children => (IDirectory) new Directory(directory.Name, children));
    }

    public static Task<Result<INode>> ToImmutable(this IMutableNode directory)
    {
        return directory switch
        {
            IMutableDirectory storageDirectory => storageDirectory.ToImmutable().Map(x => (INode)x),
            IMutableFile mutableStorageFile => mutableStorageFile.ToImmutable().Map(x => (INode)x),
            _ => throw new ArgumentOutOfRangeException(nameof(directory))
        };
    }

    public static Task<Result<IFile>> ToImmutable(this IMutableFile mutableFile)
    {
        return mutableFile.GetContents()
            .Map(data => (IFile)new File(mutableFile.Name, data));
    }
}