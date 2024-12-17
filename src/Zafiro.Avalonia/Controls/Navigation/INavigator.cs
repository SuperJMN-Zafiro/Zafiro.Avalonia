using System.Reactive;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Controls.Navigation;

public interface INavigator
{
    IEnhancedCommand Back { get; }

    object Content { get; set; }

    void Go(Func<INavigator, object> target);
    
    public void Go(Func<object> target)
    {
        Go(_ => target());
    }
}