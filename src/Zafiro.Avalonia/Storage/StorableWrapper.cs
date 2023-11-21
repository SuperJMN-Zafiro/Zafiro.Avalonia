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

    public Task<Result<long>> Size()
    {
        return Result
            .Try(file.GetBasicPropertiesAsync)
            .Map(properties => (long?)properties.Size)
            .EnsureNotNull("Size not available");
    }

    public Task<Result<bool>> Exists()
    {
        return Task.FromResult(Result.Success(true));
    }

    public Task<Result<Stream>> GetContents(CancellationToken cancellationToken = default)
    {
        return Result.Try(() => file.OpenReadAsync());
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            using (var openWriteAsync = await file.OpenWriteAsync())
            {
                await stream.CopyToAsync(openWriteAsync, cancellationToken);
            }
        });
    }

    public Task<Result> Delete(CancellationToken cancellationToken = default)
    {
        return Result.Try(file.DeleteAsync);
    }

    public bool IsHidden => false;
}