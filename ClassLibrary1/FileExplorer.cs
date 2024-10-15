using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace ClassLibrary1;

public partial class FileExplorer : ReactiveObject, IFileExplorer
{
    [Reactive] private string address;

    public FileExplorer(IMutableFileSystem fileSystem, Func<IRooted<IMutableDirectory>, IFileExplorer, IDirectoryContents> getContents)
    {
        Navigate = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(Address).Map(directory => getContents(new Rooted<IMutableDirectory>(Address, directory), this)));
        Address = fileSystem.InitialPath;
        Contents = Navigate.Successes();
    }

    public IObservable<IDirectoryContents> Contents { get; }

    public ReactiveCommand<Unit, Result<IDirectoryContents>> Navigate { get; }
    public async Task<Result> GoTo(ZafiroPath newAddress)
    {
        Address = newAddress;
        var result = await Navigate.Execute();
        return result.Map(_ => Unit.Default);
    }
}