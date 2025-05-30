using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Controls.Shell;

public abstract class MyRoutedEventTriggerBase<T> : RoutedEventTriggerBase where T : RoutedEventArgs
{
    private IDisposable? _disposable;

    /// <summary>
    /// 
    /// </summary>
    protected abstract RoutedEvent<T> RoutedEvent { get; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
            var disposable = interactive.AddDisposableHandler(
                RoutedEvent,
                Handler,
                EventRoutingStrategy);
            return disposable;
        }

        return DisposableAction.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Handler(object? sender, T e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Execute(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void Execute(T e)
    {
        e.Handled = MarkAsHandled;
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}

public class MyPointerReleasedTrigger : MyRoutedEventTriggerBase<PointerReleasedEventArgs>
{
    static MyPointerReleasedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerReleasedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override RoutedEvent<PointerReleasedEventArgs> RoutedEvent
        => InputElement.PointerReleasedEvent;
}