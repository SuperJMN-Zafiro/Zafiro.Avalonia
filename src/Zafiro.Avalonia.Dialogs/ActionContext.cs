namespace Zafiro.UI.Avalonia;

public class ActionContext<T>
{
    public T ViewModel{ get; set; }
    public IWindow Window { get; set; }
}