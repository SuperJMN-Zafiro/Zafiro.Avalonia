using System.Reactive.Linq;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core.Toolbar;

public class ToolBarViewModel : ReactiveValidationObject
{
    public ExplorerContext Context { get; }
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> currentDirectory;

    public ToolBarViewModel(ExplorerContext context)
    {
        Context = context;
        currentDirectory = context.Directory.ToProperty(this, x => x.CurrentDirectory);
        
        CreateDirectory = ReactiveCommand.CreateFromTask(async () =>
        {
            var createDirectoryViewModel = new CreateDirectoryViewModel(context);
            var result = await context.Dialog.ShowAndGetResult(createDirectoryViewModel, "Create directory", x => x.IsValid(), model => model.DirectoryName, Maybe<Action<ConfigureSizeContext>>.From(x =>
            {
                x.Width = 300;
                x.Height = double.NaN;
            }));
            
            return await result.Map(name => CurrentDirectory.RootedDir.Value.CreateSubdirectory(name));
        });

        CreateDirectory.Values().HandleErrorsWith(context.NotificationService);
    }

    public ReactiveCommand<Unit, Maybe<Result<IMutableDirectory>>> CreateDirectory { get; set; }


    public DirectoryContentsViewModel CurrentDirectory => currentDirectory.Value;
    public IObservable<bool> IsPasting => Observable.Return(false);
}