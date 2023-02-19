using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.UI;

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

    public IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params FileTypeFilter[] filters)
    {
        var dialog = new OpenFileDialog { AllowMultiple = true };
        return PickCore(dialog, filters);
    }

    public IObservable<Result<IZafiroFile>> PickSingle(params FileTypeFilter[] filters)
    {
        var dialog = new OpenFileDialog { AllowMultiple = false };
        return PickCore(dialog, filters).Select(x => x.First());
    }

    private IObservable<IEnumerable<Result<IZafiroFile>>> PickCore(OpenFileDialog dialog, params FileTypeFilter[] filters)
    {
        dialog.Filters = FilePicker.Map(filters);

        return Observable.FromAsync(() => dialog.ShowAsync(parent))
            .WhereNotNull()
            .Select(strings => strings.Select(s => fileSystem.GetFile(s)));
    }
}