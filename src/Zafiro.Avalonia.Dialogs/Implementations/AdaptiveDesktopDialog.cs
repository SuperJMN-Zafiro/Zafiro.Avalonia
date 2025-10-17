using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Zafiro.Avalonia.Dialogs.Views;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Dialogs.Implementations;

/// <summary>
/// Desktop dialog implementation with adaptive sizing based on content measurement.
/// </summary>
public class AdaptiveDesktopDialog : IDialog
{
    private readonly AdaptiveDialogSizer.SizingConfig? sizingConfig;
    private readonly IDialogSizingStrategy sizingStrategy;

    public AdaptiveDesktopDialog(
        AdaptiveDialogSizer.SizingConfig? sizingConfig = null,
        IDialogSizingStrategy? sizingStrategy = null)
    {
        this.sizingConfig = sizingConfig;
        this.sizingStrategy = sizingStrategy ?? new ContentMeasurementSizingStrategy();
    }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var showTask = await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var mainWindow = ApplicationUtils.MainWindow().GetValueOrThrow("Cannot get the main window");

            var window = new Window
            {
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Icon = mainWindow.Icon,
            };

            var closeable = new CloseableWrapper(window);
            var options = optionsFactory(closeable);

            var dialogContent = new DialogControl
            {
                Content = viewModel,
                Options = options
            };

            window.Content = dialogContent;

            // Calculate size using strategy
            var parentBounds = mainWindow.Bounds;
            var availableSize = new Size(parentBounds.Width, parentBounds.Height);
            var config = sizingConfig ?? new AdaptiveDialogSizer.SizingConfig();

            var finalSize = sizingStrategy.Calculate(viewModel, availableSize, config);

            // Apply size
            window.Width = finalSize.Width;
            window.Height = finalSize.Height;
            window.MinWidth = config.MinWidth;
            window.MinHeight = config.MinHeight;
            window.MaxWidth = config.FixedWidth ?? (availableSize.Width * config.MaxWidthRatio);
            window.MaxHeight = config.FixedHeight ?? (availableSize.Height * config.MaxHeightRatio);
            window.SizeToContent = SizeToContent.Manual;

            var result = await window.ShowDialog<bool?>(mainWindow).ConfigureAwait(false);
            return result is not (null or false);
        });

        return showTask;
    }
}