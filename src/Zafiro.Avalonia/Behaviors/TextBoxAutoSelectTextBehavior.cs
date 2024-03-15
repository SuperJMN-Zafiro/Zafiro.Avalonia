using System.Reactive.Disposables;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;
using JetBrains.Annotations;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Behaviors;

[PublicAPI]
public class TextBoxAutoSelectTextBehavior : AttachedToVisualTreeBehavior<TextBox>
{
    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var gotFocus = AssociatedObject.OnEvent(InputElement.GotFocusEvent);
        var lostFocus = AssociatedObject.OnEvent(InputElement.LostFocusEvent);
        var isFocused = gotFocus.Select(_ => true).Merge(lostFocus.Select(_ => false));

        isFocused
            .Throttle(TimeSpan.FromSeconds(0.1))
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(focused => focused)
            .Do(_ => AssociatedObject.SelectAll())
            .Subscribe()
            .DisposeWith(disposable);
    }
}