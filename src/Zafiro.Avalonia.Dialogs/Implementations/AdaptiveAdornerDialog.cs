using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs.Implementations;

/// <summary>
/// Adorner-based dialog implementation with adaptive sizing based on content measurement.
/// </summary>
public class AdaptiveAdornerDialog : IDialog, ICloseable
{
    private readonly Lazy<AdornerLayer> adornerLayerLazy;
    private readonly Stack<Control> dialogs = new();
    private readonly AdaptiveDialogSizer.SizingConfig? sizingConfig;
    private readonly IDialogSizingStrategy sizingStrategy;

    private TaskCompletionSource<bool>? currentDialog;

    public AdaptiveAdornerDialog(
        Func<AdornerLayer> getAdornerLayer,
        AdaptiveDialogSizer.SizingConfig? sizingConfig = null,
        IDialogSizingStrategy? sizingStrategy = null)
    {
        adornerLayerLazy = new Lazy<AdornerLayer>(() => getAdornerLayer());
        this.sizingConfig = sizingConfig;
        this.sizingStrategy = sizingStrategy ?? new ContentMeasurementSizingStrategy();
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

            var dialogContent = new DialogControl
            {
                Content = viewModel,
                Options = options,
            };

            var dialog = new DialogViewContainer
            {
                Title = title,
                Content = dialogContent,
                Close = ReactiveCommand.Create(() => Dismiss()),
            };

            var adornerLayer = adornerLayerLazy.Value;

            // Get available size from adorner layer parent
            var availableSize = (adornerLayer.Parent as Visual)?.Bounds.Size ?? new Size(800, 600);
            var config = sizingConfig ?? new AdaptiveDialogSizer.SizingConfig();

            // Calculate size using strategy
            var finalSize = sizingStrategy.Calculate(viewModel, availableSize, config);

            // Apply size
            dialog.Width = finalSize.Width;
            dialog.Height = finalSize.Height;
            dialog.MinWidth = config.MinWidth;
            dialog.MinHeight = config.MinHeight;
            dialog.MaxWidth = config.FixedWidth ?? (availableSize.Width * config.MaxWidthRatio);
            dialog.MaxHeight = config.FixedHeight ?? (availableSize.Height * config.MaxHeightRatio);
            dialog.HorizontalAlignment = HorizontalAlignment.Center;
            dialog.VerticalAlignment = VerticalAlignment.Center;

            adornerLayer.Children.Add(dialog);
            dialogs.Push(dialog);

            return currentDialog.Task;
        });

        return showTask;
    }
}