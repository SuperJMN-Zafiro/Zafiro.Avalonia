using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public interface IFileExplorer
{
    Task<Result> GoTo(ZafiroPath newAddress);
}