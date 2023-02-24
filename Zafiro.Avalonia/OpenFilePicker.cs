using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia;

public class OpenFilePicker : IOpenFilePicker
{
    private readonly TopLevel parent;

    public OpenFilePicker(TopLevel parent)
    {
        this.parent = parent;
    }

    private IObservable<IEnumerable<IStorable>> PickCore(FilePickerOpenOptions filePickerOpenOptions, params FileTypeFilter[] filters)
    {
        return Observable
            .FromAsync(() => parent.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions))
            .Select(list => list.Select(file => new StorableWrapper(file)));
    }

    public IObservable<IEnumerable<IStorable>> PickMultiple(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = FilePicker.Map(filters)
        });
    }

    public IObservable<Maybe<IStorable>> PickSingle(params FileTypeFilter[] filters)
    {
        return PickCore(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = FilePicker.Map(filters)
        }).Select(x => x.TryFirst());
    }
}