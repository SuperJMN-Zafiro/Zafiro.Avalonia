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
    public Maybe<Action<ConfigureSizeContext>> ConfigureWindowAction { get; }

    public SimpleDesktopDialogService()
    {
    }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory, Maybe<Action<ConfigureSizeContext>> configureDialogAction)
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
        configureDialogAction.Or(DefaultWindowConfigurator).Execute(configure => ConfigureSize(configure, window, MainWindow));

        var result = await window.ShowDialog<bool?>(MainWindow).ConfigureAwait(false);
        return result is not (null or false);
    }

    private static void ConfigureSize(Action<ConfigureSizeContext> action, Window dialog, Window parent)
    {
        var configureSizeContext = new ConfigureSizeContext()
        {
            ParentBounds = parent.Bounds
        };

        action(configureSizeContext);
        if (double.IsNaN(configureSizeContext.Width) && double.IsNaN(configureSizeContext.Height))
        {
            dialog.SizeToContent = SizeToContent.WidthAndHeight;
        }

        else if (double.IsNaN(configureSizeContext.Width))
        {
            dialog.SizeToContent = SizeToContent.Width;
            dialog.Height = configureSizeContext.Height;
        }

        else if (double.IsNaN(configureSizeContext.Height))
        {
            dialog.SizeToContent = SizeToContent.Height;
            dialog.Width = configureSizeContext.Width;
        }
        else
        {
            dialog.Height = configureSizeContext.Height;
            dialog.Width = configureSizeContext.Width;
        }
    }

    private static Action<ConfigureSizeContext> DefaultWindowConfigurator()
    {
        return context =>
        {
            context.Width = context.ParentBounds.Width / 3;
            context.Height = context.ParentBounds.Height / 3;
        };
    }

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}