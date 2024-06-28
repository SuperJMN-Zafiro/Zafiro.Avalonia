using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Readonly;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class StorageSampleViewModel
{
    public StorageSampleViewModel(IFileSystemPicker storage)
    {
        OpenFile = ReactiveCommand.CreateFromTask(async () =>
        {
            Result<Maybe<IFile>> pickForOpen = await storage.PickForOpen(new FileTypeFilter("All files", new[] { "*.*" }));
            return pickForOpen.IgnoreResult();
        });
        
        SelectedPaths = OpenFile.Values().Select(x => x.Name);
    }

    public IObservable<string> SelectedPaths { get; }

    public ReactiveCommand<Unit, Maybe<IFile>> OpenFile { get; }
}