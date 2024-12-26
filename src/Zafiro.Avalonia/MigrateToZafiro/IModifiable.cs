using System.Reactive;

namespace Zafiro.Avalonia.MigrateToZafiro;

public interface IModifiable
{
    IObservable<Unit> Modified { get; }
}