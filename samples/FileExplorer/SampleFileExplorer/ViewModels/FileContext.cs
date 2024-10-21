using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using ClassLibrary1;
using DynamicData;

namespace SampleFileExplorer.ViewModels;

public class FileContext : IDisposable
{
    private readonly SourceCache<IFileExplorer, string> explorers;
    readonly CompositeDisposable disposables = new CompositeDisposable();

    public FileContext()
    {
        explorers = new SourceCache<IFileExplorer, string>(x => x.Key).DisposeWith(disposables);;

        explorers
            .Connect()
            .Bind(out var collection)
            .Subscribe()
            .DisposeWith(disposables);

        Explorers = collection;
    }

    public ReadOnlyObservableCollection<IFileExplorer> Explorers { get; set; }

    public void Add(IFileExplorer fileExplorer)
    {
        explorers.AddOrUpdate(fileExplorer);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}