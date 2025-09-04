using System.Reactive.Disposables;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Zafiro.Avalonia.ViewLocators;

namespace Zafiro.Avalonia.Behaviors
{
    public class PointerPressedOutsideTriggerBehavior : StyledElementTrigger
    {
        private readonly CompositeDisposable disposables = new();

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();

            if (!(AssociatedObject is Visual target))
                return;

            var topLevel = TopLevel.GetTopLevel(target);

            topLevel?.OnEvent(InputElement.PointerPressedEvent)
                .Where(_ => IsEnabled)
                .Where(evt =>
                {
                    if (evt.EventArgs.Source is not Visual src)
                    {
                        return true;
                    }

                    return !target.GetVisualDescendants().Contains(src);
                })
                .Subscribe(evt =>
                {
                    Interaction.ExecuteActions(this, Actions, null);
                    evt.EventArgs.Handled = true;
                }).DisposeWith(disposables);
        }

        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            disposables.Clear();
        }
    }
}