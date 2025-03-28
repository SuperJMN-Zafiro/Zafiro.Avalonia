using System.Reactive.Linq;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Dialogs;

public class Option<T, Q>(string title, IEnhancedCommand<T, Q> command, Settings settings) : IOption<T, Q>
{
    public string Title => title;
    public IEnhancedCommand Command => command;
    public bool IsDefault => settings.IsDefault;
    public bool IsCancel => settings.IsCancel;
    public IObservable<bool> IsVisible => settings.IsVisible;
    public OptionRole Role { get; set; } = settings.Role;
    public IEnhancedCommand<T, Q> TypedCommand => command;
}

public class Option : IOption
{
    public Option(string title, IEnhancedCommand command, Settings settings)
    {
        Title = title;
        Command = command;
        IsDefault = settings.IsDefault;
        IsCancel = settings.IsCancel;
        IsVisible = settings.IsVisible;
        Role = settings.Role;
    }

    public string Title { get; }
    public IEnhancedCommand Command { get; }
    public bool IsDefault { get; }
    public bool IsCancel { get; }
    public IObservable<bool> IsVisible { get; }
    public OptionRole Role { get; }
}