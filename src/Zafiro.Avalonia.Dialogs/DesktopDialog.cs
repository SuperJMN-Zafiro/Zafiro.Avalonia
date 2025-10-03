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

            var sizePlan = DialogSizing.For(mainWindow);

            var window = new Window
            {
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Icon = mainWindow.Icon,
                SizeToContent = SizeToContent.WidthAndHeight,
            };

            DialogSizing.Apply(window, sizePlan);

            var closeable = new CloseableWrapper(window);
            var options = optionsFactory(closeable);

            var dialogControl = new DialogControl
            {
                Content = viewModel,
                Options = options
            };

            DialogSizing.Apply(dialogControl, sizePlan);

            window.Content = dialogControl;

            var result = await window.ShowDialog<bool?>(mainWindow).ConfigureAwait(false);
            return result is not (null or false);
        });

        return showTask;
    }
}