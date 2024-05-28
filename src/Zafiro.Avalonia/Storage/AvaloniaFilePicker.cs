using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.Storage;

[PublicAPI]
public class AvaloniaFilePicker : IFilePicker
{
    private readonly IStorageProvider storageProvider;

    public AvaloniaFilePicker(IStorageProvider storageProvider)
    {
        this.storageProvider = storageProvider;
    }

    public IObservable<IEnumerable<IZafiroFile>> PickForOpenMultiple(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = FilePicker.Map(filters)
        });
    }

    public IObservable<Maybe<IZafiroFile>> PickForOpen(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = FilePicker.Map(filters)
        }).Select(x => x.TryFirst());
    }

    public IObservable<Maybe<IZafiroFile>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters)
    {
        return Observable
            .FromAsync(() => storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = FilePicker.Map(filters),
                DefaultExtension = defaultExtension.GetValueOrDefault(),
                SuggestedFileName = desiredName
            }))
            .Select(file => Maybe.From<IZafiroFile>(file is null ? default! : new StorableWrapper(file)));
    }

    private IObservable<IEnumerable<IZafiroFile>> PickCore(FilePickerOpenOptions filePickerOpenOptions)
    {
        return Observable
            .FromAsync(() => storageProvider.OpenFilePickerAsync(filePickerOpenOptions))
            .Select(list => list.Select(file => new StorableWrapper(file)));
    }

    public async Task<Maybe<IEnumerable<IMutableDirectory>>> PickFolder()
    {
        var openFolderPickerAsync = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions());
        return Maybe.From(openFolderPickerAsync.AsEnumerable()).MapEach(x =>
        {
            return (IMutableDirectory) new StorageDirectory(x);
        });
    }
}