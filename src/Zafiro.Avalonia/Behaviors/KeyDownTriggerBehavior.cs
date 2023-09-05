using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Behaviors;

[PublicAPI]
public class KeyDownTriggerBehavior : Trigger
{
    public static readonly StyledProperty<RoutingStrategies> EventRoutingStrategyProperty = AvaloniaProperty.Register<KeyDownTriggerBehavior, RoutingStrategies>(nameof(EventRoutingStrategy));

    public static readonly StyledProperty<Key> KeyProperty = AvaloniaProperty.Register<KeyDownTriggerBehavior, Key>(nameof(Key));

    private CompositeDisposable disposables = new();

    public RoutingStrategies EventRoutingStrategy
    {
        get => GetValue(EventRoutingStrategyProperty);
        set => SetValue(EventRoutingStrategyProperty, value);
    }

    public Key Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public bool MarkAsHandled { get; set; }

    protected override void OnAttached()
    {
        if (AssociatedObject is InputElement element)
        {
            element
                .AddDisposableHandler(InputElement.KeyDownEvent, OnKeyDown, EventRoutingStrategy)
                .DisposeWith(disposables);
        }
    }

    protected override void OnDetaching()
    {
        disposables.Dispose();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key)
        {
            e.Handled = MarkAsHandled;
            Interaction.ExecuteActions(AssociatedObject, Actions, null);
        }
    }
}