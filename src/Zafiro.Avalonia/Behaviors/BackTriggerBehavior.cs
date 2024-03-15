using System.Reactive.Disposables;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Behaviors;

[PublicAPI]
public class BackTriggerBehavior : Trigger<Visual>
{
    public static readonly StyledProperty<bool> MarkAsHandledProperty = AvaloniaProperty.Register<BackTriggerBehavior, bool>(nameof(MarkAsHandled));
    private readonly CompositeDisposable disposables = new();

    public bool MarkAsHandled
    {
        get => GetValue(MarkAsHandledProperty);
        set => SetValue(MarkAsHandledProperty, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        var tl = TopLevel.GetTopLevel(AssociatedObject);

        if (tl != null)
        {
            tl.OnEvent(TopLevel.BackRequestedEvent)
                .Do(x => Handle(x.EventArgs))
                .Subscribe()
                .DisposeWith(disposables);
        }

        base.OnAttachedToVisualTree();
    }

    protected override void OnDetaching()
    {
        disposables.Dispose();
    }

    private void Handle(RoutedEventArgs routedEventArgs)
    {
        routedEventArgs.Handled = MarkAsHandled;
        Interaction.ExecuteActions(this, Actions, null);
    }
}