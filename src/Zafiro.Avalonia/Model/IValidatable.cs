namespace Zafiro.Avalonia.Model;

public interface IValidatable
{
    public IObservable<bool> IsValid { get; }
}