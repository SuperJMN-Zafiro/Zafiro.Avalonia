using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia;

public static class Mixin
{
    public static IObservable<EventPattern<TEventArgs>> OnEvent<TEventArgs>(this IInteractive target, RoutedEvent<TEventArgs> routedEvent, RoutingStrategies routingStrategies) where TEventArgs : RoutedEventArgs
    {
        return Observable.FromEventPattern<TEventArgs>(
            add => target.AddHandler(routedEvent, add, routingStrategies),
            remove => target.RemoveHandler(routedEvent, remove));
    }
}