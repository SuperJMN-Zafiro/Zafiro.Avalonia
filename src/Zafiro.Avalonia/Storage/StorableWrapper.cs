using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.Storage;

internal class StorableWrapper : IZafiroFile
{
    private readonly IStorageFile file;

    public StorableWrapper(IStorageFile file)
    {
        this.file = file;
    }

    public IObservable<byte> Contents => throw new NotImplementedException();

    Task<Result<bool>> IZafiroFile.Exists => Task.FromResult(Result.Success(true));

    public ZafiroPath Path => file.Path.ToString();
    public Task<Result<FileProperties>> Properties
    {
        get
        {
            return Result.Success().Map(() => file.GetBasicPropertiesAsync()).Map(properties => new FileProperties(false, DateTimeOffset.Now, (long) properties.Size));
        }
    }

    public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes => throw new NotSupportedException("Hashes is not implemented yet");
    public IFileSystemRoot FileSystem => throw new NotSupportedException("Cannot get filesystem of IStorageFile");
    public Task<Result> Delete() => throw new NotImplementedException();

    public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result<Stream>> GetData() => Result.Try(() => file.OpenReadAsync());

    public Task<Result> SetData(Stream stream, CancellationToken cancellationToken)
    {
        var task = Result
            .Try(() => file.OpenWriteAsync())
            .Map(destination => stream.CopyToAsync(destination, cancellationToken));
        
        return task;
    }
}