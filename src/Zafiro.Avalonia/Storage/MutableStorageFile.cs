using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

internal class MutableStorageFile : IMutableFile
{
    public MutableStorageFile(IStorageFile storageFile)
    {
        StorageFile = storageFile;
    }

    public IStorageFile StorageFile { get; }

    public string Name => StorageFile.Name;
    public async Task<Result> SetContents(IData data)
    {
        await using var stream = await StorageFile.OpenReadAsync();
        return await data.DumpTo(stream);
    }
}