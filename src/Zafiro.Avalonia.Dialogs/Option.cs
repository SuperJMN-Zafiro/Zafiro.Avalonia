using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public class Option<T, Q>(string title, IEnhancedCommand<T, Q> command, bool isDefault = false, bool isCancel = false) : IOption<T, Q>
{
    public string Title => title;
    public IEnhancedCommand Command => command;
    public bool IsDefault => isDefault;
    public bool IsCancel => isCancel;
    public IEnhancedCommand<T, Q> TypedCommand => command;
}

public class Option : IOption
{
    public Option(string title, IEnhancedCommand command, bool isDefault = false, bool isCancel = false)
    {
        Title = title;
        Command = command;
        IsDefault = isDefault;
        IsCancel = isCancel;
    }

    public string Title { get; }
    public IEnhancedCommand Command { get; }
    public bool IsDefault { get; }
    public bool IsCancel { get; }
}