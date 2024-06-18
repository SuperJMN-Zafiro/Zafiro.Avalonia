using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Obsolete;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class SimpleDesktopDialogService : ISimpleDialog
{
    public Maybe<Action<ConfigureWindowContext>> ConfigureWindowAction { get; }

    public SimpleDesktopDialogService(Maybe<Action<ConfigureWindowContext>> configureWindowAction)
    {
        ConfigureWindowAction = configureWindowAction;
    }

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
            //SizeToContent = SizeToContent.WidthAndHeight,
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

#if DEBUG        
        window.AttachDevTools();
#endif
        ConfigureWindowAction.Or(DefaultWindowConfigurator).Execute(configure => configure(new ConfigureWindowContext(MainWindow, window)));

        var result = await window.ShowDialog<bool?>(MainWindow).ConfigureAwait(false);
        return result != null;
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