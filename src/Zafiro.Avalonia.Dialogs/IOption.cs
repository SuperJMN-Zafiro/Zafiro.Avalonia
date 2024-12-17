using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public interface IOption<T, Q> : IOption
{
    IEnhancedCommand<T, Q> TypedCommand { get; }
}

public interface IOption
{
    string Title { get; }

    IEnhancedCommand Command { get; }

    bool IsDefault { get; }

    bool IsCancel { get; }
    public IObservable<bool> IsVisible { get; }
}