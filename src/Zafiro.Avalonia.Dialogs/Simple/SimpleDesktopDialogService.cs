using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class SimpleDesktopDialogService : ISimpleDialog
{
    public Maybe<Action<ConfigureWindowContext>> ConfigureWindowAction { get; }

    public SimpleDesktopDialogService(Maybe<Action<ConfigureWindowContext>> configureWindowAction)
    {
        ConfigureWindowAction = configureWindowAction;
    }

    public Task Show(object viewModel, string title, IObservable<bool> canSubmit)
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
            SizeToContent = SizeToContent.WidthAndHeight,
            Icon = MainWindow.Icon,
        };

        window.Content = new DialogViewContainer()
        {
            Classes = { "Desktop" },
            Content = new DialogView
            {
                DataContext = new DialogViewModel(viewModel,
                    new Option("OK", ReactiveCommand.Create(() => window.Close(), canSubmit)))
            }
        };

#if DEBUG        
        window.AttachDevTools();
#endif
        ConfigureWindowAction.Or(DefaultWindowConfigurator).Execute(configure => configure(new ConfigureWindowContext(MainWindow, window)));

        return window.ShowDialog(MainWindow);
    }

    private static Action<ConfigureWindowContext> DefaultWindowConfigurator()
    {
        return context =>
        {
            context.ToConfigure.Width = context.Parent.Bounds.Width / 3;
            context.ToConfigure.Height = context.Parent.Bounds.Width / 3;
        };
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}