namespace Zafiro.Avalonia.Wizard.Interfaces;

public interface IValidatable
{
    public IObservable<bool> IsValid { get; }
}