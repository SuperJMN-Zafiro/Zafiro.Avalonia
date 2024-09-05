using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.Misc;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core;

public class SelectionContext : ReactiveObject, ISelectionHandler
{
    private readonly CompositeDisposable disposables = new();
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit,Unit>> selectAll;
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit,Unit>> selectNone;

    public SelectionContext(IObservable<DirectoryContentsViewModel> directories)
    {
        var selectionChanges = directories
            .Select(x => new ObservableSelectionModel<IDirectoryItem, string>(x.Selection, item => item.Key))
            .DisposePrevious()
            .Select(x => x.Selection)
            .Switch();

        SelectionCount = selectionChanges.Count();
        TotalCount = directories.Select(x => x.Entries).Switch().Count();
        
        SelectionChanges = selectionChanges;
        selectAll = directories.Select(model => ReactiveCommand.Create(() => model.Selection.SelectAll())).DisposePrevious().ToProperty(this, x => x.SelectAll);
        selectNone = directories.Select(model => ReactiveCommand.Create(() => model.Selection.Clear())).DisposePrevious().ToProperty(this, x => x.SelectNone);

        // Copy = CreateCopyCommand(selectionHandler, explorerContext.Clipboard, selectedEntries);
        // Paste = CreatePasteCommand(directories, explorerContext.Clipboard, explorerContext.TransferManager);
        // Delete = CreateDeleteCommand(selectionHandler, explorerContext.TransferManager);
        //IsPasting = Paste.IsExecuting;
    }

    public IObservable<bool> IsPasting { get; }

    
    [Reactive] public bool IsTouchFriendlySelectionEnabled { get; set; }
    
    public ReactiveCommand<Unit, Unit> SelectNone => selectNone.Value;
    public ReactiveCommand<Unit, Unit> SelectAll => selectAll.Value;
    public IObservable<int> SelectionCount { get; }
    public IObservable<int> TotalCount { get; }
    public IObservable<IChangeSet<IDirectoryItem, string>> SelectionChanges { get; }
}