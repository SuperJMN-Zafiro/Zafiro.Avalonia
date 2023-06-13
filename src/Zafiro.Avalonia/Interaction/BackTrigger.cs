using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using JetBrains.Annotations;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Interaction;

[PublicAPI]
internal class BackTrigger : Trigger<Visual>
{
    public static readonly StyledProperty<bool> MarkAsHandledProperty = AvaloniaProperty.Register<BackTrigger, bool>(nameof(MarkAsHandled));
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
        global::Avalonia.Xaml.Interactivity.Interaction.ExecuteActions(this, Actions, null);
    }
}