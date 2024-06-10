using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

public class StorageDirectory : IMutableDirectory, IRooted
{
    private readonly IStorageFolder folder;

    public StorageDirectory(IStorageFolder folder)
    {
        this.folder = folder;
    }

    public async Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        var mutableChildren = await Result.Try(() => folder.GetItemsAsync())
            .Map(async a =>
            {
                var storageItems = await a.ToListAsync().ConfigureAwait(false);
                return storageItems.AsEnumerable();
            })
            .MapEach(ToMutableNode).ConfigureAwait(false);
            
        return mutableChildren;
    }

    private async Task<IMutableNode> ToMutableNode(IStorageItem item)
    {
        return item switch
        {
            IStorageFile storageFile => new MutableStorageFile(storageFile),
            IStorageFolder storageFolder => new StorageDirectory(storageFolder),
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };
    }

    public string Name => folder.Name;
    public Task<Result<IEnumerable<INode>>> Children() => MutableChildren().Map(x => x.Cast<INode>());
    public ZafiroPath Path => folder.Path.ToString();
}