using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs;

public abstract class DialogService : IDialogService
{
    public object GetFinalContent(object content)
    {
        var type = ViewModelToViewDictionary.TryFind(content.GetType());
        var finalContent = type.Match(t =>
        {
            var instance = Activator.CreateInstance(t) ?? $"Could not create instance of {t}";
            if (instance is Control c)
            {
                c.DataContext = content;
            }

            return instance;
        }, () => content);

        return finalContent;
    }

    public DialogService(IReadOnlyDictionary<Type, Type> viewModelToViewDictionary)
    {
        ViewModelToViewDictionary = viewModelToViewDictionary;
    }

    protected IReadOnlyDictionary<Type, Type> ViewModelToViewDictionary { get; }

    protected static IEnumerable<Option> CreateOptions<T>(object viewModel, IWindow window, IEnumerable<OptionConfiguration<T>> options)
    {
        return options.Select(configuration => new Option(configuration.Title, configuration.Factory(new ActionContext<T>(window, (T) viewModel))));
    }

    public abstract Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);

    public static IDialogService Create(IApplicationLifetime lifetime, IReadOnlyDictionary<Type, Type> viewModelToViewDictionary, Maybe<Action<ConfigureWindowContext>> configureWindow)
    {
        return lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime => new ClassicDesktopDialogService(viewModelToViewDictionary, configureWindow),
            ISingleViewApplicationLifetime { MainView: not null } singleView => new SingleViewDialogService(singleView.MainView, viewModelToViewDictionary),
            ISingleViewApplicationLifetime { MainView: null } => throw new InvalidOperationException("Please, set MainView in the Application.Current.Lifetime before calling this method"),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
        };
    }
}