using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core.Navigator;

public class PathNavigatorViewModel : ReactiveObject, IPathNavigator
{
    private readonly ObservableAsPropertyHelper<Maybe<IRooted<IMutableDirectory>>> currentDirectory;

    public PathNavigatorViewModel(IMutableFileSystem mutableFileSystem, INotificationService notificationService)
    {
        LoadRequestedPath = ReactiveCommand.CreateFromTask(() => RequestedPath.Map(path => mutableFileSystem.GetDirectory(path).Map(directory => (IRooted<IMutableDirectory>)Rooted.Create(path, directory))));
        LoadRequestedPath.HandleErrorsWith(notificationService);
        IsNavigating = LoadRequestedPath.IsExecuting;
        History = new History();
        LoadRequestedPath.Successes().Select(x => x.Path).Select(Maybe.From).BindTo(this, x => x.History.CurrentFolder);
        GoBack = History.GoBack;
        
        this.WhenAnyValue(x => x.History.CurrentFolder).Values()
            .Do(path => RequestedPathString = path)
            .ToSignal()
            .InvokeCommand(LoadRequestedPath);
        
         
        Directories = LoadRequestedPath.Successes().Select(Maybe.From);
        Directories
            .Do(maybe => maybe.Execute(x => RequestedPathString = x.Path.ToString()))
            .Subscribe();

        currentDirectory = Directories.ToProperty(this, x => x.CurrentDirectory);
        
        RequestedPathString = string.Empty;
        GoUp = ReactiveCommand.Create(() => SetAndLoad(CurrentDirectory.Value.Path.Parent().ToString()), Directories.Values().Select(rooted => rooted.Path.Parent().HasValue));
    }

    public Maybe<IRooted<IMutableDirectory>> CurrentDirectory => currentDirectory.Value;

    private Result<ZafiroPath> RequestedPath => RequestedPathString.Trim() == "" ? Result.Success(ZafiroPath.Empty) : ZafiroPath.Create(RequestedPathString!);

    public ReactiveCommandBase<Unit, Result<IRooted<IMutableDirectory>>> LoadRequestedPath { get; }

    public IObservable<Maybe<IRooted<IMutableDirectory>>> Directories { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; }
    public ReactiveCommand<Unit, Unit> GoUp { get; }

    [Reactive]
    public string RequestedPathString { get; set; }

    private History History { get; }

    public IObservable<bool> IsNavigating { get; }

    public void SetAndLoad(ZafiroPath requestedPath)
    {
        History.CurrentFolder = requestedPath;
    }
}