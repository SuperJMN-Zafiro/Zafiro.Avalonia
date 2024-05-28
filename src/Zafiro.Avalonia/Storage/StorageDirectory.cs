using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

public class StorageDirectory : IMutableDirectory
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
                var storageItems = await a.ToListAsync();
                return storageItems.AsEnumerable();
            })
            .MapEach(ToMutableNode);
            
        return mutableChildren;
    }

    private IMutableNode ToMutableNode(IStorageItem item)
    {
        return item switch
        {
            IStorageFile storageFile => throw new NotImplementedException(),
            IStorageFolder storageFolder => (IMutableNode)new StorageDirectory(storageFolder),
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };
    }
}

public class MutableFile
{
    public MutableFile(IStorageItem item)
    {
    }
}
