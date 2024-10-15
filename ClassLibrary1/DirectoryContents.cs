using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public class DirectoryContents : IDirectoryContents, IDisposable
{
    private readonly IMutableDirectory directory;
    private readonly CompositeDisposable disposable = new();
    
    public DirectoryContents(IMutableDirectory directory)
    {
        this.directory = directory;
        var watcher = new DirectoryWatcher(directory);
        
        watcher.StartWatching().DisposeWith(disposable);
        
        watcher.Items
            .Transform(DirectoryItem)
            .Sort(SortExpressionComparer<IDirectoryItem>.Descending(p => p is DirectoryViewModel)
                .ThenByAscending(p => p.Name))
            .Bind(out var itemsCollection)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        Items = itemsCollection;
    }
    
    private IDirectoryItem DirectoryItem(IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => new DirectoryItem(directory, mutableDirectory),
            IMutableFile mutableFile => new FileItem(directory, mutableFile),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
    
    public IEnumerable<IDirectoryItem> Items { get; }
    public IEnumerable<IDirectoryItem> SelectedItems { get; }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}