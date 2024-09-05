using System.Reactive.Concurrency;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

internal class MutableStorageFile : IMutableFile, IRooted
{
    public MutableStorageFile(IStorageFile storageFile)
    {
        StorageFile = storageFile;
    }

    public IStorageFile StorageFile { get; }

    public string Name => StorageFile.Name;

    public Task<Result> SetContents(IData data, IScheduler? scheduler = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IData>> GetContents()
    {
        return await Result.Try(async () =>
        {
            var size = MaybeEx.FromNullableStruct((await StorageFile.GetBasicPropertiesAsync().ConfigureAwait(false)).Size);

            var openReadAsync = StorageFile.OpenReadAsync;
            return size.Match(s => Data.FromStream(openReadAsync, (long)s), () => new Data(Observable.Empty<byte[]>(), 0));
        }).ConfigureAwait(false);
    }

    public Task<Result> SetContents(IData data, CancellationToken cancellationToken = default, IScheduler? scheduler = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Delete()
    {
        throw new NotImplementedException();
    }

    public bool IsHidden { get; }

    public Task<Result> Create()
    {
        throw new NotImplementedException();
    }

    public ZafiroPath Path => StorageFile.Path.ToString();

    public async Task<Result> SetContents(IData data)
    {
        var stream = await StorageFile.OpenWriteAsync().ConfigureAwait(false);
        await using var stream1 = stream.ConfigureAwait(false);
        return await data.DumpTo(stream).ConfigureAwait(false);
    }
}