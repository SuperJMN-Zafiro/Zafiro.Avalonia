using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Interfaces;
using Zafiro.Core.Mixins;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace TestApp.ViewModels;

public class StorageSampleViewModel
{
    public StorageSampleViewModel(IStorage storage)
    {
        OpenFile = ReactiveCommand.CreateFromObservable(() => storage.PickForOpen(new FileTypeFilter("All files", new[] { "*.*" })));
        SelectedPaths = OpenFile.Values().Select(x => x.Name);
    }

    public IObservable<string> SelectedPaths { get; set; }

    public ReactiveCommand<Unit, Maybe<IStorable>> OpenFile { get; }
}