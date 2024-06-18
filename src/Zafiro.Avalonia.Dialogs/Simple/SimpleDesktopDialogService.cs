using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Zafiro.Avalonia.Dialogs.Obsolete;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class SimpleDesktopDialogService : ISimpleDialog
{
    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory)
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var window = new Window
        {
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Icon = MainWindow.Icon,
        };
        
        var closeable = new CloseableWrapper(window);
        var options = optionsFactory(closeable);
        window.Content = new DialogViewContainer()
        {
            Classes = { "Desktop" },
            Content = new DialogView
            {
                DataContext = new DialogViewModel(viewModel, options)
            }
        };

        SetWindowSize(window);

#if DEBUG        
        window.AttachDevTools();
#endif
        var result = await window.ShowDialog<bool?>(MainWindow).ConfigureAwait(false);
        return result is not (null or false);
    }

    private void SetWindowSize(Window window)
    {
        window.Height = MainWindow.Bounds.Height / 3;
        window.Width = MainWindow.Bounds.Width / 3;
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}