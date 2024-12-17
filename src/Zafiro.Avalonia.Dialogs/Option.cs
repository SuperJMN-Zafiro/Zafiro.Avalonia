using System.Reactive.Linq;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public class Option<T, Q>(string title, IEnhancedCommand<T, Q> command, bool isDefault = false, bool isCancel = false, IObservable<bool>? isVisible = null) : IOption<T, Q>
{
    public string Title => title;
    public IEnhancedCommand Command => command;
    public bool IsDefault => isDefault;
    public bool IsCancel => isCancel;
    
    public IObservable<bool> IsVisible => isVisible ?? Observable.Return(true);
    public IEnhancedCommand<T, Q> TypedCommand => command;
}

public class Option : IOption
{
    public Option(string title, IEnhancedCommand command, bool isDefault = false, bool isCancel = false, IObservable<bool>? isVisible = null)
    {
        Title = title;
        Command = command;
        IsDefault = isDefault;
        IsCancel = isCancel;
        IsVisible = isVisible ?? Observable.Return(true);
    }

    public string Title { get; }
    public IEnhancedCommand Command { get; }
    public bool IsDefault { get; }
    public bool IsCancel { get; }
    public IObservable<bool> IsVisible { get; }
}