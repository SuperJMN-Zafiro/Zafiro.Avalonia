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

    public async Task<Maybe<IMutableFile>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters)
    {
        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            FileTypeChoices = FilePicker.Map(filters),
            DefaultExtension = defaultExtension.GetValueOrDefault(),
            SuggestedFileName = desiredName
        });
        
        return Maybe.From<IMutableFile>(file is null ? default! : new StorageFile(file));
    }

    private IObservable<IEnumerable<IZafiroFile>> PickCore(FilePickerOpenOptions filePickerOpenOptions)
    {
        return Observable
            .FromAsync(() => storageProvider.OpenFilePickerAsync(filePickerOpenOptions))
            .Select(list => list.Select(file => new StorableWrapper(file)));
    }

    public Task<Maybe<IMutableDirectory>> PickFolder()
    {
        return PickFolders(new FolderPickerOpenOptions { AllowMultiple = false }).Bind(x => x.TryFirst());
    }

    public async Task<Maybe<IEnumerable<IMutableDirectory>>> PickFolders(FolderPickerOpenOptions folderPickerOpenOptions)
    {
        var openFolderPickerAsync = await storageProvider.OpenFolderPickerAsync(folderPickerOpenOptions);
        return Maybe.From(openFolderPickerAsync.AsEnumerable()).MapEach(x => (IMutableDirectory)new StorageDirectory(x));
    }
}