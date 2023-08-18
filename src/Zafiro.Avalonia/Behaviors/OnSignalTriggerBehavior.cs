using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Zafiro.Avalonia.Behaviors;

public class OnSignalTriggerBehavior : Trigger
{
    public OnSignalTriggerBehavior()
    {
        this.WhenAnyObservable(x => x.Trigger)
            .Do(_ => Interaction.ExecuteActions(this, Actions, null))
            .Subscribe();
    }

    public static readonly StyledProperty<IObservable<Unit>> TriggerProperty = AvaloniaProperty.Register<OnSignalTriggerBehavior, IObservable<Unit>>(
        nameof(Trigger));

    public IObservable<Unit> Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }
}