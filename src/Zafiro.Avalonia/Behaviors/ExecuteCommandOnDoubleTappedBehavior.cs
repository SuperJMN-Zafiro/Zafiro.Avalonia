﻿using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public class ExecuteCommandOnDoubleTappedBehavior : DisposingBehavior<Control>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ExecuteCommandOnDoubleTappedBehavior, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        return Gestures.DoubleTappedEvent.AddClassHandler<InputElement>(
            (x, _) =>
            {
                if (Equals(x, AssociatedObject))
                {
                    if (Command is { } cmd && cmd.CanExecute(default))
                    {
                        cmd.Execute(default);
                    }
                }
            },
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }
}