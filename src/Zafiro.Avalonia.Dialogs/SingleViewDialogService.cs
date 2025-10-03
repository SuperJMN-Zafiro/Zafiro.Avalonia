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

            var dialogControl = new DialogControl
            {
                Content = viewModel,
                Options = options,
            };

            var dialog = new DialogViewContainer
            {
                Title = title,
                Content = dialogControl,
                Close = ReactiveCommand.Create(() => Dismiss()),
            };

            var adornerLayer = adornerLayerLazy.Value;

            var boundsObservable = adornerLayer.Parent!
                .GetObservable(Visual.BoundsProperty);

            var layoutObservable = boundsObservable
                .Select(DialogSizePolicy.Calculate);

            dialog[!Layoutable.HeightProperty] = boundsObservable
                .Select(rect => rect.Height)
                .ToBinding();

            dialog[!Layoutable.WidthProperty] = boundsObservable
                .Select(rect => rect.Width)
                .ToBinding();

            dialogControl[!Layoutable.WidthProperty] = layoutObservable
                .Select(layout => layout.PreferredContent.Width)
                .ToBinding();

            dialogControl[!Layoutable.HeightProperty] = layoutObservable
                .Select(layout => layout.PreferredContent.Height)
                .ToBinding();

            dialogControl[!Layoutable.MaxWidthProperty] = layoutObservable
                .Select(layout => layout.MaximumContent.Width)
                .ToBinding();

            dialogControl[!Layoutable.MaxHeightProperty] = layoutObservable
                .Select(layout => layout.MaximumContent.Height)
                .ToBinding();

            dialogControl[!Layoutable.MinWidthProperty] = layoutObservable
                .Select(layout => layout.MinimumContent.Width)
                .ToBinding();

            dialogControl[!Layoutable.MinHeightProperty] = layoutObservable
                .Select(layout => layout.MinimumContent.Height)
                .ToBinding();

            adornerLayer.Children.Add(dialog);
            dialogs.Push(dialog);

            return currentDialog.Task;
        });

        return showTask;
    }
}