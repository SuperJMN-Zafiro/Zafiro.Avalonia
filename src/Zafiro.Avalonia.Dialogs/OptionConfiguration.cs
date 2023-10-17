using System.Windows.Input;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

public class OptionConfiguration<T, TResult> where T : IResult<TResult>
{
    public string Title { get; }
    public Func<T, ICommand> Factory { get; }

    public OptionConfiguration(string title, Func<T, ICommand> factory)
    {
        Title = title;
        Factory = factory;
    }
}