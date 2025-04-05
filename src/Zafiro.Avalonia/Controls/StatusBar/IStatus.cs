namespace Zafiro.Avalonia.Controls.StatusBar;

public interface IStatus
{
    IObservable<object> Statuses { get; }
    void Push(object status);
}