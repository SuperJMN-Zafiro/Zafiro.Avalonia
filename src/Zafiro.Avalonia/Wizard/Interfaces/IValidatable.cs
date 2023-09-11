namespace Zafiro.Avalonia.NewWizard.Interfaces;

public interface IValidatable
{
    public IObservable<bool> IsValid { get; }
}