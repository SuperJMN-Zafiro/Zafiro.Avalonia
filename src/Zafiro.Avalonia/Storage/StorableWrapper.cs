using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.Storage;

internal class StorableWrapper : IZafiroFile
{
    private readonly IStorageFile file;

    public StorableWrapper(IStorageFile file)
    {
        this.file = file;
    }

    public ZafiroPath Path => file.Path.ToString();

    public Task<long> Size()
    {
        throw new NotSupportedException();
    }

    public Task<Result<Stream>> GetContents()
    {
        return Result.Try(file.OpenReadAsync);
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(async () =>
        {
            using (var openWriteAsync = await file.OpenWriteAsync())
            {
                await stream.CopyToAsync(openWriteAsync);
            }
        });
    }

    public Task<Result> Delete()
    {
        return Result.Try(file.DeleteAsync);
    }
}