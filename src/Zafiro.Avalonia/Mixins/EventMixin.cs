using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Mixins
{
    public static class EventMixin
    {
        public static IObservable<EventPattern<TEventArgs>> OnEvent<TEventArgs>(
            this Interactive target,
            RoutedEvent<TEventArgs> routedEvent,
            RoutingStrategies routingStrategies = RoutingStrategies.Bubble) where TEventArgs : RoutedEventArgs
        {
            return Observable.FromEventPattern<TEventArgs>(
                add => target.AddHandler(routedEvent, add, routingStrategies),
                remove => target.RemoveHandler(routedEvent, remove));
        }
    }
}