using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
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
        var stream = await StorageFile.OpenWriteAsync().ConfigureAwait(false);
        await using var stream1 = stream.ConfigureAwait(false);
        return await data.DumpTo(stream).ConfigureAwait(false);
    }

    public async Task<Result<IData>> GetContents()
    {
        return await Result.Try(async () =>
        {
            var size = ResultEx.FromNullableStruct((await StorageFile.GetBasicPropertiesAsync().ConfigureAwait(false)).Size);
            
            Func<Task<Stream>> openReadAsync = StorageFile.OpenReadAsync;
            return size.Match(arg => (IData)new Data(openReadAsync.Chunked(), (long) arg), () => new Data(Observable.Empty<byte[]>(), 0));
        }).ConfigureAwait(false);
    }
}