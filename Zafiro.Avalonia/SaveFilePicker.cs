using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia;

public class SaveFilePicker : ISaveFilePicker
{
    private readonly TopLevel parent;

    public SaveFilePicker(TopLevel parent)
    {
        this.parent = parent;
    }

    public IObservable<Maybe<IStorable?>> Pick(string desiredName, string defaultExtension, params FileTypeFilter[] filters)
    {
        return Observable
            .FromAsync(() => parent.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = FilePicker.Map(filters),
                DefaultExtension = defaultExtension,
                SuggestedFileName = desiredName,
            }))
            .Select(file => Maybe.From(file is null ? null : (IStorable)new StorableWrapper(file)));
    }
}