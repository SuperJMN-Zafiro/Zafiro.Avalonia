using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Navigator;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public partial class FileExplorer : ReactiveObject, IFileExplorer
{
    private readonly IMutableFileSystem fileSystem;
    private readonly Func<IRooted<IMutableDirectory>, IFileExplorer, IDirectoryContents> getContents;
    [Reactive] private string address;
    private Stack<ZafiroPath> history = new();
    ISubject<bool> canGoBack = new Subject<bool>();

    public FileExplorer(IMutableFileSystem fileSystem, Func<IRooted<IMutableDirectory>, IFileExplorer, IDirectoryContents> getContents)
    {
        this.fileSystem = fileSystem;
        this.getContents = getContents;
        LoadAddress = ReactiveCommand.CreateFromTask(() => GoToCore(Address));

        LoadAddress.Successes().Do(contents =>
        {
            history.Push(contents.Path);
            canGoBack.OnNext(history.Count > 1);
        }).Subscribe();
        
        Address = fileSystem.InitialPath;

        GoBack = ReactiveCommand.CreateFromTask(async () =>
        {
            history.Pop();
            canGoBack.OnNext(history.Count > 1);
            var goToCore = await GoToCore(history.Peek());
            return goToCore
                .Tap(() => Address = history.Peek());
        }, canGoBack);
        
        Contents = LoadAddress.Merge(GoBack).Successes();
        Items = this.WhenAnyObservable(explorer => explorer.Contents).Select(x => x.Items)
            .EditDiff(x => x.Key);
    }

    public IObservable<IChangeSet<IDirectoryItem, string>> Items { get; }

    public async Task<Result<IDirectoryContents>> GoTo(ZafiroPath path)
    {
        Address = path;
        return await LoadAddress.Execute();
    }

    private Task<Result<IDirectoryContents>> GoToCore(ZafiroPath path)
    {
        return fileSystem
            .GetDirectory(path)
            .Map(directory => getContents(new Rooted<IMutableDirectory>(path, directory), this));
    }
    
    public ReactiveCommand<Unit, Result<IDirectoryContents>> GoBack { get; }

    public IObservable<IDirectoryContents> Contents { get; }

    public ReactiveCommand<Unit, Result<IDirectoryContents>> LoadAddress { get; }
}