using System.Windows.Input;

namespace Zafiro.Avalonia.Dialogs;

public class OptionConfiguration<T, TResult> where T : IHaveResult<TResult>
{
    public string Title { get; }
    public Func<T, ICommand> Factory { get; }

    public OptionConfiguration(string title, Func<T, ICommand> factory)
    {
        Title = title;
        Factory = factory;
    }
}