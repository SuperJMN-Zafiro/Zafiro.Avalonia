using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs;

public class AdornerDialog : IDialog, ICloseable
{
    private readonly Lazy<AdornerLayer> adornerLayerLazy;
    private readonly Stack<Control> dialogs = new();

    private TaskCompletionSource<bool>? currentDialog;

    public AdornerDialog(Func<AdornerLayer> getAdornerLayer)
    {
        adornerLayerLazy = new Lazy<AdornerLayer>(() => getAdornerLayer());
    }


    public void Close()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (dialogs.Count > 0)
            {
                var dialog = dialogs.Pop();
                adornerLayerLazy.Value.Children.Remove(dialog);
            }

            currentDialog?.TrySetResult(true);
            currentDialog = null;
        });
    }

    public void Dismiss()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (dialogs.Count > 0)
            {
                var dialog = dialogs.Pop();
                adornerLayerLazy.Value.Children.Remove(dialog);
            }

            currentDialog?.TrySetResult(false);
            currentDialog = null;
        });
    }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var showTask = await Dispatcher.UIThread.InvokeAsync(() =>
        {
            currentDialog = new TaskCompletionSource<bool>();
            var options = optionsFactory(this);

            var adornerLayer = adornerLayerLazy.Value;
            if (adornerLayer.Parent is not Visual parent)
            {
                throw new InvalidOperationException("The adorner layer must have a parent visual to calculate dialog sizing.");
            }

            var initialPlan = DialogSizing.For(parent.Bounds.Size);

            var dialogControl = new DialogControl
            {
                Content = viewModel,
                Options = options,
            };

            DialogSizing.Apply(dialogControl, initialPlan);

            var dialog = new DialogViewContainer
            {
                Title = title,
                Content = dialogControl,
                Close = ReactiveCommand.Create(() => Dismiss()),
            };

            DialogSizing.Apply(dialog, initialPlan);

            var planStream = parent
                .GetObservable(Visual.BoundsProperty)
                .Select(rect => DialogSizing.For(rect.Size))
                .StartWith(initialPlan)
                .Publish()
                .RefCount();

            dialog[!Layoutable.MaxWidthProperty] = planStream.Select(plan => plan.MaxWidth).ToBinding();
            dialog[!Layoutable.MaxHeightProperty] = planStream.Select(plan => plan.MaxHeight).ToBinding();
            dialog[!Layoutable.MinWidthProperty] = planStream.Select(plan => plan.MinWidth).ToBinding();
            dialog[!Layoutable.MinHeightProperty] = planStream.Select(plan => plan.MinHeight).ToBinding();
            dialog[!ContentControl.PaddingProperty] = planStream.Select(plan => new Thickness(plan.Padding)).ToBinding();
            dialog[!Layoutable.MarginProperty] = planStream.Select(plan => new Thickness(plan.OuterMargin)).ToBinding();

            dialogControl[!Layoutable.MaxWidthProperty] = planStream.Select(plan => plan.ContentMaxWidth).ToBinding();
            dialogControl[!Layoutable.MaxHeightProperty] = planStream.Select(plan => plan.ContentMaxHeight).ToBinding();
            dialogControl[!Layoutable.MinWidthProperty] = planStream.Select(plan => plan.ContentMinWidth).ToBinding();
            dialogControl[!Layoutable.MinHeightProperty] = planStream.Select(plan => plan.ContentMinHeight).ToBinding();

            adornerLayer.Children.Add(dialog);
            dialogs.Push(dialog);

            return currentDialog.Task;
        });

        return showTask;
    }
}