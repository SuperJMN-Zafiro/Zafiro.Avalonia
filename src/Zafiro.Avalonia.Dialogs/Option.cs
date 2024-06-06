using System.Windows.Input;

namespace Zafiro.Avalonia.Dialogs;

public class Option
{
    public Option(string title, ICommand command, bool isDefault = false, bool isCancel = false)
    {
        Title = title;
        Command = command;
        IsDefault = isDefault;
        IsCancel = isCancel;
    }

    public string Title { get; }
    public ICommand Command { get; }
    public bool IsDefault { get; }
    public bool IsCancel { get; }
}