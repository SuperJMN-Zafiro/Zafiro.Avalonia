namespace Zafiro.UI.Avalonia;

public abstract class DialogServiceBase : IDialogService
{
    protected static IEnumerable<Option> CreateOptions<T>(object viewModel, IWindow window, IEnumerable<OptionConfiguration<T>> options)
    {
        return options.Select(configuration => new Option(configuration.Title, configuration.Factory(new ActionContext<T>
        {
            Window =  window,
            ViewModel = (T) viewModel,
        })));
    }

    public abstract Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);
}