using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

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

    public IObservable<Result<IZafiroFile>> Pick(params (string, string[])[] filters)
    {
        var dialog = new SaveFileDialog();
        dialog.Filters = filters.Select(tuple => new FileDialogFilter
        {
            Extensions = tuple.Item2.ToList(),
            Name = tuple.Item1
        }).ToList();

        return Observable.FromAsync(() => dialog.ShowAsync(parent))
            .WhereNotNull()
            .Select(path => fileSystem.GetFile(path));
    }
}