namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public interface IBusy
{
    public IObservable<bool> IsBusy { get; }
}