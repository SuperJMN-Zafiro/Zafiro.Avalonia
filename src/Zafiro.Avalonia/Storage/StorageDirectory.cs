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
        return await Result.Try(() => folder.GetItemsAsync())
            .Map(async a =>
            {
                var storageItems = await a.ToListAsync().ConfigureAwait(false);
                return storageItems.AsEnumerable();
            })
            .MapEach(ToMutableNode)
            .Map(Task.WhenAll)
            .Map(nodes => nodes.AsEnumerable());
    }

    public Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableFile>> Get(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteFile(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteSubdirectory(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<IMutableNode>>> GetChildren(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> HasFile(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> HasSubdirectory(string name)
    {
        throw new NotImplementedException();
    }

    public IObservable<IMutableFile> FileCreated { get; }
    public IObservable<IMutableDirectory> DirectoryCreated { get; }
    public IObservable<string> FileDeleted { get; }
    public IObservable<string> DirectoryDeleted { get; }

    public Task<Result> Delete()
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
    public Task<Result> Create()
    {
        throw new NotImplementedException();
    }
}