using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using ReactiveUI;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.Avalonia.Storage;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class StorageSampleViewModel
{
    public StorageSampleViewModel(IFileSystemPicker storage)
    {
        OpenFile = ReactiveCommand.CreateFromTask(async () =>
        {
            Result<Maybe<IFile>> result = await storage.PickForOpen(new FileTypeFilter("All files", new[] { "*.jpg", "*.png", "*.gif", "*.bmp" }));

            return result.Map(maybe => maybe.Map(file => file)).GetValueOrDefault();
        });

        var files = OpenFile.Values().Publish().RefCount();
        
        SelectedPaths = files.Select(file => file.Name);
        SelectedBytes = files.Select(file => file.Bytes());
    }

    public IObservable<byte[]> SelectedBytes { get; set; }

    public IObservable<string> SelectedPaths { get; }

    public ReactiveCommand<Unit, Maybe<IFile>> OpenFile { get; }
}