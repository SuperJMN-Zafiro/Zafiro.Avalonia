using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

public interface IDialog2
{
    public Task Show(object viewModel, string title, IObservable<bool> canSubmit);
}

public interface IHaveCommit
{
    IObservable<Unit> Commited { get; }
}

public class DesktopDialogService2 : IDialog2
{
    private readonly Maybe<Action<ConfigureWindowContext>> configureWindowAction;

    public DesktopDialogService2(Maybe<Action<ConfigureWindowContext>> configureWindowAction)
    {
        this.configureWindowAction = configureWindowAction;
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