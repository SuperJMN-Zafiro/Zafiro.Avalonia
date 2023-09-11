namespace Zafiro.Avalonia.Dialogs.Obsolete;

public class ActionContext<T>
{
    public ActionContext(ICloseable closeable, T viewModel)
    {
        ViewModel = viewModel;
        Closeable = closeable;
    }

    public T ViewModel { get; set; }
    public ICloseable Closeable { get; set; }
}