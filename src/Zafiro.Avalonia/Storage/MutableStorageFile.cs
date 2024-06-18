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

    public async Task<Result> SetContents(IData data)
    {
        var stream = await StorageFile.OpenWriteAsync().ConfigureAwait(false);
        await using var stream1 = stream.ConfigureAwait(false);
        return await data.DumpTo(stream).ConfigureAwait(false);
    }

    public Task<Result> SetContents(IData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IData>> GetContents()
    {
        return await Result.Try(async () =>
        {
            var size = MaybeEx.FromNullableStruct((await StorageFile.GetBasicPropertiesAsync().ConfigureAwait(false)).Size);

            var openReadAsync = StorageFile.OpenReadAsync;
            return size.Match(arg => (IData) new Data(openReadAsync.Chunked(), (long) arg), () => new Data(Observable.Empty<byte[]>(), 0));
        }).ConfigureAwait(false);
    }

    public Task<Result> Delete()
    {
        throw new NotImplementedException();
    }

    public ZafiroPath Path => StorageFile.Path.ToString();
    public bool IsHidden { get; }
}