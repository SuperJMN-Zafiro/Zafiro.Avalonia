using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia;

public class DesktopOpenFilePicker : IOpenFilePicker
{
    private readonly IZafiroFileSystem fileSystem;
    private readonly Window parent;

    public DesktopOpenFilePicker(Window parent, IZafiroFileSystem fileSystem)
    {
        this.parent = parent;
        this.fileSystem = fileSystem;
    }

    public IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params (string, string[])[] filters)
    {
        var dialog = new OpenFileDialog { AllowMultiple = true };
        return PickCore(dialog, filters);
    }

    public IObservable<Result<IZafiroFile>> PickSingle(params (string, string[])[] filters)
    {
        var dialog = new OpenFileDialog { AllowMultiple = false };
        return PickCore(dialog, filters).Select(x => x.First());
    }

    private IObservable<IEnumerable<Result<IZafiroFile>>> PickCore(OpenFileDialog dialog, params (string, string[])[] filters)
    {
        dialog.Filters = filters.Select(tuple => new FileDialogFilter
        {
            Extensions = tuple.Item2.ToList(),
            Name = tuple.Item1
        }).ToList();

        return Observable.FromAsync(() => dialog.ShowAsync(parent))
            .WhereNotNull()
            .Select(strings => strings.Select(s => fileSystem.GetFile(s)));
    }
}