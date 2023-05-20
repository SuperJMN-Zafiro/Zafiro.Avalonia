using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Zafiro.UI.Avalonia;

public class ClassicDesktopDialogService : DialogServiceBase
{
    public override Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options)
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var window = new Window
        {
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
        };

        var wrapper = new WindowWrapper(window);

        window.Content = new DialogView { DataContext = new DialogViewModel(viewModel, title, CreateOptions(viewModel, wrapper, options).ToArray()) };
        
        return window.ShowDialog(MainWindow);
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow;
}