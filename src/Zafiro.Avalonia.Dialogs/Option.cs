using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.UI.Avalonia;

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