namespace Zafiro.Avalonia.Dialogs;

public class ActionContext<T>
{
    public ActionContext(IWindow window, T viewModel)
    {
        ViewModel = viewModel;
        Window = window;
    }

    public T ViewModel{ get; set; }
    public IWindow Window { get; set; }
}