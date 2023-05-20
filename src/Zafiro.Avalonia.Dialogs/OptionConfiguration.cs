using System.Windows.Input;

namespace Zafiro.UI.Avalonia;

public class OptionConfiguration<TViewModel>
{
    public string Title { get; }
    public Func<ActionContext<TViewModel>, ICommand> Factory { get; }

    public OptionConfiguration(string title, Func<ActionContext<TViewModel>, ICommand> factory)
    {
        Title = title;
        Factory = factory;
    }
}