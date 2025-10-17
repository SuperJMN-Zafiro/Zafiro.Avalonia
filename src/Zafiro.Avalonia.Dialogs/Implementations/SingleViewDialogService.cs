using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs.Implementations;

/// <summary>
/// Legacy adorner-based dialog without adaptive sizing.
/// Use <see cref="AdaptiveAdornerDialog"/> for adaptive sizing instead.
/// </summary>
[Obsolete("Use AdaptiveAdornerDialog for better content-aware sizing. This class will be removed in a future version.")]
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

            var dialog = new DialogViewContainer
            {
                Title = title,
                Content = new DialogControl()
                {
                    Content = viewModel,
                    Options = options,
                },
                Close = ReactiveCommand.Create(() => Dismiss()),
            };

            var adornerLayer = adornerLayerLazy.Value;

            dialog[!Layoutable.HeightProperty] = adornerLayer.Parent!
                .GetObservable(Visual.BoundsProperty)
                .Select(rect => rect.Height)
                .ToBinding();

            dialog[!Layoutable.WidthProperty] = adornerLayer.Parent!
                .GetObservable(Visual.BoundsProperty)
                .Select(rect => rect.Width)
                .ToBinding();

            adornerLayer.Children.Add(dialog);
            dialogs.Push(dialog);

            return currentDialog.Task;
        });

        return showTask;
    }
}