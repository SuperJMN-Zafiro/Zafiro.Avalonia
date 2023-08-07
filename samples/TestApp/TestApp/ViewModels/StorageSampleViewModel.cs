using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class StorageSampleViewModel
{
    public StorageSampleViewModel(IFilePicker storage)
    {
        OpenFile = ReactiveCommand.CreateFromObservable(() => storage.PickForOpen(new FileTypeFilter("All files", new[] { "*.*" })));
        SelectedPaths = OpenFile.Values().Select(x => x.Path.Name());
    }

    public IObservable<string> SelectedPaths { get; }

    public ReactiveCommand<Unit, Maybe<IZafiroFile>> OpenFile { get; }
}