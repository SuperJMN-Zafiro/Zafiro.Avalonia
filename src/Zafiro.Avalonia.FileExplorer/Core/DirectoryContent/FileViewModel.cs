using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

public class FileViewModel : IDirectoryItem
{
    private readonly Subject<Unit> deleteSubject = new();
    public IMutableFile File { get; }

    public FileViewModel(IMutableDirectory parent, IMutableFile file)
    {
        File = file;
        Delete = ReactiveCommand.CreateFromTask(() =>
        {
            return parent.DeleteFile(file.Name).Tap(() => deleteSubject.OnNext(Unit.Default));
        });
    }

    public string Name => File.Name;
    public ReactiveCommand<Unit, Result> Delete { get; }
    public string Key => Name;
    public IObservable<Unit> Deleted => deleteSubject.AsObservable();
}