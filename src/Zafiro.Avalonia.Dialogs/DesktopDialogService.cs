using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.MigrateToZafiro;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialogService : IDialogService
{
    private readonly Maybe<Action<ConfigureWindowContext>> configureWindow;

    public DesktopDialogService(Maybe<Action<ConfigureWindowContext>> configureWindow)
    {
        this.configureWindow = configureWindow;
    }

    public async Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, params OptionConfiguration<TViewModel, TResult>[] options) where TViewModel : UI.IResult<TResult>
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

    private static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
}