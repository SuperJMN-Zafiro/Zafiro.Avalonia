using System.Reactive;
using System.Reactive.Linq;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public class Settings
{
    public Settings(bool isDefault = false, bool isCancel = false, IObservable<bool>? isVisible = null, bool autoAdvance = false)
    {
        IsDefault = isDefault;
        IsCancel = isCancel;
        IsVisible = isVisible ?? Observable.Return(true);
        AutoAdvance = autoAdvance;
    }

    public bool IsDefault { get; init;}
    public bool IsCancel { get; init;}
    public IObservable<bool> IsVisible { get; init;}
    public bool AutoAdvance { get; init; }
    public OptionRole Role { get; init; }
}

public static class OptionBuilder
{
    public static Option<T, Q> Create<T, Q>(string title, IEnhancedCommand<T, Q> command, Settings settings)
    {
        return new Option<T, Q>(title, command, settings);
    }

    public static Option<Unit, Unit> Create<T, Q>(string title, IEnhancedCommand<Unit, Unit> command, Settings settings)
    {
        return new Option<Unit, Unit>(title, command, settings);
}

    public static Option Create(string title, IEnhancedCommand command, Settings settings)
    {
        return new Option(title, command, settings);
    }
}