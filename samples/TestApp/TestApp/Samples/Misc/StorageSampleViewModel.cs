using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using Zafiro.UI;

namespace TestApp.Samples.Misc;

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