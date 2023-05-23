using System.Windows.Input;

namespace Zafiro.Avalonia.Dialogs;

public class Option
{
    public Option(string title, ICommand command)
    {
        Title = title;
        Command = command;
    }

    public string Title { get; }
    public ICommand Command { get; }
}