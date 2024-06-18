using System.Reactive.Subjects;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;

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

    public Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableFile>> CreateFile(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableDirectory>> CreateDirectory(string name)
    {
        throw new NotImplementedException();
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
    public bool IsHidden => false;
}