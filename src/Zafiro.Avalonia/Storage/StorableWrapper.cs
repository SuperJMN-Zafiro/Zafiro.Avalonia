using System.Reactive.Linq;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.IO;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Storage;

internal class StorableWrapper : IZafiroFile
{
    private readonly IStorageFile file;

    public StorableWrapper(IStorageFile file)
    {
        this.file = file;
    }

    Task<Result<bool>> IZafiroFile.Exists => Task.FromResult(Result.Success(true));

    public ZafiroPath Path => file.Path.ToString();
    public Task<Result<FileProperties>> Properties => Result.Try(() => file.GetBasicPropertiesAsync()).Map(props => new FileProperties(false, props.DateCreated ?? DateTimeOffset.MinValue, GetSize(props)));

    public Task<Result> SetContents(IObservable<byte> contents)
    {
        return Result.Try(async () =>
        {
            await using var stream = await file.OpenWriteAsync();
            await contents.DumpTo(stream);
        });
    }

    public IObservable<byte> Contents => ObservableFactory.UsingAsync(() => file.OpenReadAsync(), stream => stream.ToObservable());

    public Task<Result> Delete() => Result.Try(file.DeleteAsync);

    private static long GetSize(StorageItemProperties props)
    {
        if (props.Size is { } s)
        {
            return (long) s;
        }

        return 0;
    }
}