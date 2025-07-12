using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Binding;

namespace Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

public class DirectoryContentsViewModel : ViewModelBase, IDisposable
{
    private readonly CompositeDisposable disposable = new();

    public DirectoryContentsViewModel(IRooted<IMutableDirectory> rootedDir,
        ExplorerContext context)
    {
        RootedDir = rootedDir;
        Context = context;

        var watcher = new DirectoryWatcher(rootedDir.Value);
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

    public IRooted<IMutableDirectory> RootedDir { get; }
    public ExplorerContext Context { get; }

    public IObservable<IChangeSet<IDirectoryItem, string>> Entries { get; }

    public ReadOnlyObservableCollection<IDirectoryItem> Items { get; }

    public SelectionModel<IDirectoryItem> Selection { get; } = new() { SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }

    private IDirectoryItem DirectoryItem(IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => new DirectoryViewModel(RootedDir, mutableDirectory, Context),
            IMutableFile mutableFile => new FileViewModel(RootedDir.Value, mutableFile),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
}