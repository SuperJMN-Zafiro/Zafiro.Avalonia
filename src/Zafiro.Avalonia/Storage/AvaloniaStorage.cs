using System.Reactive.Linq;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Avalonia.Interfaces;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.Storage;

[PublicAPI]
public class AvaloniaStorage : IStorage
{
    private readonly IStorageProvider storageProvider;

    public AvaloniaStorage(IStorageProvider storageProvider)
    {
        this.storageProvider = storageProvider;
    }

    public IObservable<IEnumerable<IStorable>> PickForOpenMultiple(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = FilePicker.Map(filters)
        });
    }

    public IObservable<Maybe<IStorable>> PickForOpen(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = FilePicker.Map(filters)
        }).Select(x => x.TryFirst());
    }

    public IObservable<Maybe<IStorable>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters)
    {
        return Observable
            .FromAsync(() => storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = FilePicker.Map(filters),
                DefaultExtension = defaultExtension.GetValueOrDefault(),
                SuggestedFileName = desiredName
            }))
            .Select(file => Maybe.From<IStorable>(file is null ? default! : new StorableWrapper(file)));
    }

    private IObservable<IEnumerable<IStorable>> PickCore(FilePickerOpenOptions filePickerOpenOptions)
    {
        return Observable
            .FromAsync(() => storageProvider.OpenFilePickerAsync(filePickerOpenOptions))
            .Select(list => list.Select(file => new StorableWrapper(file)));
    }
}