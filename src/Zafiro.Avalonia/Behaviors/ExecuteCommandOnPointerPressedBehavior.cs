using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Behaviors;

public class ExecuteCommandOnPointerButtonPressedBehavior : DisposingBehavior<InputElement>
{
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategyProperty = AvaloniaProperty.Register<ExecuteCommandOnPointerPressedBehavior, RoutingStrategies>(nameof(RoutingStrategy), RoutingStrategies.Bubble);

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ExecuteCommandOnPointerPressedBehavior, ICommand?>(
            nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ExecuteCommandOnPointerPressedBehavior, object?>(
            nameof(CommandParameter));

    public static readonly StyledProperty<MouseButton> ButtonProperty =
        AvaloniaProperty.Register<ExecuteCommandOnPointerPressedBehavior, MouseButton>(nameof(Button), MouseButton.Left);

    public RoutingStrategies RoutingStrategy
    {
        get => GetValue(RoutingStrategyProperty);
        set => SetValue(RoutingStrategyProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public MouseButton Button
    {
        get => GetValue(ButtonProperty);
        set => SetValue(ButtonProperty, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject == null)
        {
            return Disposable.Empty;
        }

        var pointerPressed = AssociatedObject.OnEvent(InputElement.PointerPressedEvent, RoutingStrategy);

        var buttonPressed = pointerPressed.Where(x =>
            x.EventArgs.GetCurrentPoint(AssociatedObject).Properties.IsButtonPressed(Button));
        var command = this.WhenAnyValue(x => x.Command).WhereNotNull();


        var buttonWithCommand = buttonPressed
            .Do(pattern => pattern.EventArgs.Pointer.Capture(AssociatedObject))
            .WithLatestFrom(command);

        var executionRequest = buttonWithCommand.Select(_ => CommandParameter);

        return executionRequest.Subscribe(o =>
        {
            if (Command!.CanExecute(o))
            {
                Command.Execute(o);
            }
        });
    }
}