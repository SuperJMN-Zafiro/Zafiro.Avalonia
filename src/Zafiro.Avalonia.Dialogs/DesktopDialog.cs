using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Zafiro.Avalonia.Dialogs.Views;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialog : IDialog
{
    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var showTask = await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var mainWindow = ApplicationUtils.MainWindow().GetValueOrThrow("Cannot get the main window");

            var layout = DialogSizePolicy.Calculate(new Rect(mainWindow.Bounds.Size));

            var window = new Window
            {
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Icon = mainWindow.Icon,
                SizeToContent = SizeToContent.WidthAndHeight,
                Width = layout.PreferredWindow.Width,
                Height = layout.PreferredWindow.Height,
                MaxWidth = layout.MaximumWindow.Width,
                MaxHeight = layout.MaximumWindow.Height,
                MinWidth = layout.MinimumWindow.Width,
                MinHeight = layout.MinimumWindow.Height
            };

            var closeable = new CloseableWrapper(window);
            var options = optionsFactory(closeable);

            window.Content = new DialogControl
            {
                Content = viewModel,
                Options = options,
                Width = layout.PreferredContent.Width,
                Height = layout.PreferredContent.Height,
                MaxWidth = layout.MaximumContent.Width,
                MaxHeight = layout.MaximumContent.Height,
                MinWidth = layout.MinimumContent.Width,
                MinHeight = layout.MinimumContent.Height
            };

            var result = await window.ShowDialog<bool?>(mainWindow).ConfigureAwait(false);
            return result is not (null or false);
        });

        return showTask;
    }
}