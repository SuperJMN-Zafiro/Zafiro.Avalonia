using Avalonia.Controls.ApplicationLifetimes;

namespace Zafiro.Avalonia.Dialogs;

public abstract class DialogService : IDialogService
{
    protected static IEnumerable<Option> CreateOptions<T>(object viewModel, IWindow window, IEnumerable<OptionConfiguration<T>> options)
    {
        return options.Select(configuration => new Option(configuration.Title, configuration.Factory(new ActionContext<T>(window, (T) viewModel))));
    }

    public abstract Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);

    public static IDialogService Create(IApplicationLifetime lifetime)
    {
        return lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => new ClassicDesktopDialogService(),
            ISingleViewApplicationLifetime { MainView: not null } singleView => new SingleViewDialogService(singleView.MainView),
            ISingleViewApplicationLifetime { MainView: null } => throw new InvalidOperationException("Please, set MainView in the Application.Current.Lifetime before calling this method"),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };
    }
}