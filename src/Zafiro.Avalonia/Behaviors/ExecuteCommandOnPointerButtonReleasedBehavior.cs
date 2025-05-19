using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Behaviors;

public class ExecuteCommandOnPointerButtonReleasedBehavior : DisposingBehavior<InputElement>
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

        var releases = AssociatedObject.OnEvent(InputElement.PointerReleasedEvent, RoutingStrategy);

        var buttonReleased = releases.Where(x =>
            x.EventArgs.GetCurrentPoint(AssociatedObject).Properties.WasButtonReleased(Button));
        var command = this.WhenAnyValue(x => x.Command).WhereNotNull();

        AssociatedObject.OnEvent(InputElement.PointerCaptureLostEvent).Subscribe(pattern => { });

        var buttonWithCommand = buttonReleased
            .Do(x => x.EventArgs.Pointer.Capture(null))
            .WithLatestFrom(command);

        var executionRequest = buttonWithCommand.Select(_ => CommandParameter);

        return executionRequest
            .Subscribe(o =>
            {
                if (Command!.CanExecute(o))
                {
                    Command.Execute(o);
                }
            });
    }
}