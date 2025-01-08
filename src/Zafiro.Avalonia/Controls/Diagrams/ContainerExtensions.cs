using System.Reactive;

namespace Zafiro.Avalonia.Controls.Diagrams;

public static class ContainerExtensions
{
    public static IObservable<T> ContainerOnChanged<T>(this ItemsControl itemsControl, AvaloniaProperty<T> property)
    {
        return WhenAnyContainerCreated(itemsControl)
            .SelectMany(container =>
                container.GetObservable(property)
                    .TakeUntil(WhenContainerClearing(itemsControl, container))
            );
    }

    private static IObservable<Control> WhenAnyContainerCreated(ItemsControl itemsControl)
    {
        return Observable.FromEventPattern<ContainerPreparedEventArgs>(
                h => itemsControl.ContainerPrepared += h,
                h => itemsControl.ContainerPrepared -= h)
            .Select(pattern => pattern.EventArgs.Container);
    }

    private static IObservable<Unit> WhenContainerClearing(ItemsControl itemsControl, Control container)
    {
        return Observable.FromEventPattern<ContainerClearingEventArgs>(
                h => itemsControl.ContainerClearing += h,
                h => itemsControl.ContainerClearing -= h)
            .Where(pattern => pattern.EventArgs.Container == container)
            .Select(_ => Unit.Default);
    }
}