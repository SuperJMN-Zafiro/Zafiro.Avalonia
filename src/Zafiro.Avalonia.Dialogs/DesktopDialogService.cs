using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialogService : IDialogService
{
    private readonly Maybe<Action<ConfigureWindowContext>> configureWindowAction;

    public DesktopDialogService(Maybe<Action<ConfigureWindowContext>> configureWindowAction)
    {
        this.configureWindowAction = configureWindowAction;
    }

    public async Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, Maybe<Action<ConfigureWindowContext>> configureWindowActionOverride, params OptionConfiguration<TViewModel, TResult>[] options) where TViewModel : UI.IResult<TResult>
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

        configureWindowActionOverride
            .Or(configureWindowAction)
            .Or(DefaultWindowConfigurator)
            .Execute(action => action(new ConfigureWindowContext(MainWindow, window)));

        window.Content = new DialogViewContainer()
        {
            Classes = { "Desktop" },
            Content = new DialogView { DataContext = new DialogViewModel(viewModel, options.Select(x => new Option(x.Title, command: x.Factory(viewModel))).ToArray()) }
        };

#if DEBUG        
        window.AttachDevTools();
#endif
       
        Maybe<TResult> result = Maybe.None;
        results(viewModel).Take(1)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Do(r =>
        {
            result = Maybe.From(r);
            window.Close();
        }).Subscribe();
        
        await window.ShowDialog(MainWindow);

        return result;
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