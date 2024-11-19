using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Storage;

public class MutableStorageFile : IMutableFile, IRooted
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

    public Task<Result<IData>> GetContents()
    {
        if (RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
        {
            return GetDataWasm();
        }

        return GetDataNoWasm();
    }

    private Task<Result<IData>> GetDataWasm()
    {
        return Result.Try(async () =>
        {
            var readAsync = await StorageFile.OpenReadAsync().ConfigureAwait(false);
            var bytes = await readAsync.ReadBytes().ConfigureAwait(false);
            return Data.FromByteArray(bytes);
        });
    }

    private async Task<Result<IData>> GetDataNoWasm()
    {
        var properties = await StorageFile.GetBasicPropertiesAsync();
        var size = properties.Size;
        var maybeSize = size.AsMaybe().ToResult($"Cannot get size of a file {Name} for {GetType()}");

        return maybeSize.Map(s => Data.FromStream(StorageFile.OpenReadAsync, (long)s));
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