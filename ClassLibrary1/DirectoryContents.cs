using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public class DirectoryContents : IDirectoryContents, IDisposable
{
    private readonly IRooted<IMutableDirectory> directory;
    private readonly IFileExplorer fileExplorer;
    private readonly CompositeDisposable disposable = new();
    
    public DirectoryContents(IRooted<IMutableDirectory> directory, IFileExplorer fileExplorer)
    {
        this.directory = directory;
        this.fileExplorer = fileExplorer;
        var watcher = new DirectoryWatcher(directory.Value);
        
        watcher.StartWatching().DisposeWith(disposable);
        
        watcher.Items
            .Where(node => !node.IsHidden)
            .Transform(DirectoryItem)
            .SortAndBind(out ReadOnlyObservableCollection<IDirectoryItem> itemsCollection, SortExpressionComparer<IDirectoryItem>.Descending(p => p is IDirectoryItemDirectory)
                .ThenByAscending(p => p.Name))
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        Items = itemsCollection;
        SelectedItems = new ObservableCollection<IDirectoryItem>();
    }
    
    private IDirectoryItem DirectoryItem(IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => new DirectoryItem(directory, mutableDirectory.Name, fileExplorer),
            IMutableFile mutableFile => new FileItem(directory, mutableFile),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
    
    public IEnumerable<IDirectoryItem> Items { get; }
    public IEnumerable<IDirectoryItem> SelectedItems { get; }
    public ZafiroPath Path => directory.Path;

    public void Dispose()
    {
        // TODO release managed resources here
    }
}