using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.Interfaces;

public interface IStorage
{
    IObservable<IEnumerable<IStorable>> PickForOpenMultiple(params FileTypeFilter[] filters);
    IObservable<Maybe<IStorable>> PickForOpen(params FileTypeFilter[] filters);
    IObservable<Maybe<IStorable>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters);
}