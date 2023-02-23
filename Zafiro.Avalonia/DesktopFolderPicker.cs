using System.Reactive.Linq;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia;

public class DesktopFolderPicker : IFolderPicker
{
    private readonly Window parent;
    private readonly IZafiroFileSystem fileSystem;

    public DesktopFolderPicker(Window parent, IZafiroFileSystem fileSystem)
    {
        this.parent = parent;
        this.fileSystem = fileSystem;
    }

    public IObservable<Result<IZafiroDirectory>> Pick()
    {
        var picker = new OpenFolderDialog();
        return 
            Observable
                .FromAsync(() => picker.ShowAsync(parent))
                .Select(path => fileSystem.GetDirectory(path));
    }
}