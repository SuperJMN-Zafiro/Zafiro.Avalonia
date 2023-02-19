using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia;

public class DesktopSaveFilePicker : ISaveFilePicker
{
    private readonly IZafiroFileSystem fileSystem;
    private readonly Window parent;

    public DesktopSaveFilePicker(Window parent, IZafiroFileSystem fileSystem)
    {
        this.parent = parent;
        this.fileSystem = fileSystem;
    }

    public IObservable<Result<IZafiroFile>> Pick(params FileTypeFilter[] filters)
    {
        var dialog = new SaveFileDialog();
        dialog.Filters = FilePicker.Map(filters);

        return Observable.FromAsync(() => dialog.ShowAsync(parent))
            .WhereNotNull()
            .Select(path => fileSystem.GetFile(path));
    }
}