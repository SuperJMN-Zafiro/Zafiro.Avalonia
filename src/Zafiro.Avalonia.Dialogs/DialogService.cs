using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Dialogs;

public abstract class DialogService : IDialogService
{
    public abstract Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);

    [PublicAPI]
    public static IDialogService Create(IApplicationLifetime lifetime, IDictionary<Type, Type> viewModelToViewDictionary, Maybe<Action<ConfigureWindowContext>> configureWindow)
    {
        return lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => new ClassicDesktopDialogService(configureWindow),
            ISingleViewApplicationLifetime { MainView: not null } singleView => new SingleViewDialogService(singleView.MainView),
            ISingleViewApplicationLifetime { MainView: null } => throw new InvalidOperationException("Please, set MainView in the Application.Current.Lifetime before calling this method"),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };
    }

    protected static IEnumerable<Option> CreateOptions<T>(object viewModel, ICloseable window, IEnumerable<OptionConfiguration<T>> options)
    {
        return options.Select(configuration => new Option(configuration.Title, configuration.Factory(new ActionContext<T>(window, (T) viewModel))));
    }
}