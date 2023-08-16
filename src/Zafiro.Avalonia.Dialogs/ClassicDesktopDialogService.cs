using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs;

public class ClassicDesktopDialogService : DialogService
{
    private readonly Maybe<Action<ConfigureWindowContext>> configureWindow;

    public ClassicDesktopDialogService(IReadOnlyDictionary<Type, Type> modelToViewDictionary, Maybe<Action<ConfigureWindowContext>> configureWindow) : base(modelToViewDictionary)
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
        };

        configureWindow.Execute(action => action(new ConfigureWindowContext(MainWindow, window)));

        var wrapper = new WindowWrapper(window);
        
        window.Content = new DialogView { DataContext = new DialogViewModel(GetFinalContent(viewModel), title, CreateOptions(viewModel, wrapper, options).ToArray()) };

#if DEBUG        
        window.AttachDevTools();
#endif
        
        return window.ShowDialog(MainWindow);
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}