using System.Reactive.Subjects;

namespace Zafiro.Avalonia.Controls.StatusBar;

public class Status : IStatus
{
    private readonly ReplaySubject<object> statuses = new();

    public void Push(object status)
    {
        statuses.OnNext(status);
    }

    public Status()
    {
        Statuses = statuses;
    }

    public IObservable<object> Statuses { get; }
}