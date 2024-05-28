using Avalonia.Platform.Storage;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

internal class StorageFile : IMutableFile
{
    private readonly IStorageFile storageFile;

    public StorageFile(IStorageFile storageFile)
    {
        this.storageFile = storageFile;
    }

    public string Name => storageFile.Name;
    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}