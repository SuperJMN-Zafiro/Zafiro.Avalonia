using System.Reactive;

namespace Zafiro.Avalonia.Controls.Navigation;

public interface INavigator
{
    ReactiveCommand<Unit, Unit> Back { get; }

    object Content { get; set; }

    void Go(Func<INavigator, object> target);
    
    public void Go(Func<object> target)
    {
        Go(_ => target());
    }
}