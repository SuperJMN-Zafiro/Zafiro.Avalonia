using Avalonia.Platform.Storage;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia;

internal class StorableWrapper : IStorable
{
    private readonly IStorageFile file;

    public StorableWrapper(IStorageFile file)
    {
        this.file = file;
    }

    public Task<Stream> OpenWrite()
    {
        return file.OpenWriteAsync();
    }

    public Task<Stream> OpenRead()
    {
        return file.OpenReadAsync();
    }

    public ZafiroPath Path => file.Path.ToString();
    public string Name => file.Name;
}