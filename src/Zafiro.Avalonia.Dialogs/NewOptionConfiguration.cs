using System.Windows.Input;

namespace Zafiro.Avalonia.Dialogs;

public class NewOptionConfiguration<T, TResult> where T : IHaveResult<TResult>
{
    public string Title { get; }
    public Func<T, ICommand> Factory { get; }

    public NewOptionConfiguration(string title, Func<T, ICommand> factory)
    {
        Title = title;
        Factory = factory;
    }
}