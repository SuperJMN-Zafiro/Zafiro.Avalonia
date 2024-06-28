using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs.Obsolete;

public class ClassicDesktopDialogService : DialogService
{
    private readonly Maybe<Action<ConfigureSizeContext>> configureWindow;

    public ClassicDesktopDialogService(Maybe<Action<ConfigureSizeContext>> configureWindow)
    {
        this.configureWindow = configureWindow;
    }

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
            CanResize = true,
            SizeToContent = SizeToContent.WidthAndHeight
        };

        configureWindow.Execute(action => action(new ConfigureSizeContext()));

        var wrapper = new CloseableWrapper(window);

        window.Content = new DialogView { DataContext = new DialogViewModel(viewModel, CreateOptions(viewModel, wrapper, options).ToArray()) };

#if DEBUG        
        window.AttachDevTools();
#endif

        return window.ShowDialog(MainWindow);
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}