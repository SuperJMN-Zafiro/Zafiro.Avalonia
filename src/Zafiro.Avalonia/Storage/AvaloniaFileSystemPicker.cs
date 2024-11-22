using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Avalonia.Storage;

[PublicAPI]
public class AvaloniaFileSystemPicker : IFileSystemPicker
{
    private readonly IStorageProvider storageProvider;

    public AvaloniaFileSystemPicker(IStorageProvider storageProvider)
    {
        this.storageProvider = storageProvider;
    }

    public Task<Result<IEnumerable<IFile>>> PickForOpenMultiple(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = FilePicker.Map(filters)
        });
    }

    public Task<Result<Maybe<IFile>>> PickForOpen(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = FilePicker.Map(filters)
        }).Map(files => files.TryFirst());
    }

    public async Task<Maybe<IMutableFile>> PickForSave(string desiredName, Maybe<string> defaultExtension,
        params FileTypeFilter[] filters)
    {
        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            FileTypeChoices = FilePicker.Map(filters),
            DefaultExtension = defaultExtension.GetValueOrDefault(),
            SuggestedFileName = desiredName
        }).ConfigureAwait(false);

        return Maybe.From<IMutableFile>(file is null ? default! : new MutableStorageFile(file));
    }

    private Task<Result<IEnumerable<IFile>>> PickCore(FilePickerOpenOptions filePickerOpenOptions)
    {
        return Result.Try(async () => (await storageProvider.OpenFilePickerAsync(filePickerOpenOptions)).AsEnumerable())
            .MapEach(storageFile => new MutableStorageFile(storageFile))
            .MapEach(x => x.AsReadOnly())
            .CombineSequentially();
    }

    public Task<Maybe<IMutableDirectory>> PickFolder()
    {
        return PickFolders(new FolderPickerOpenOptions { AllowMultiple = false }).Bind(x => x.TryFirst());
    }

    public async Task<Maybe<IEnumerable<IMutableDirectory>>> PickFolders(
        FolderPickerOpenOptions folderPickerOpenOptions)
    {
        var openFolderPickerAsync =
            await storageProvider.OpenFolderPickerAsync(folderPickerOpenOptions).ConfigureAwait(false);
        return Maybe.From(openFolderPickerAsync.AsEnumerable())
            .MapEach(IMutableDirectory (x) => new StorageDirectory(x));
    }
}