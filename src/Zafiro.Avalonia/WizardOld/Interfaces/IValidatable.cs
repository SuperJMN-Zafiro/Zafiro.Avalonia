namespace Zafiro.Avalonia.WizardOld.Interfaces;

public interface IValidatable
{
    public IObservable<bool> IsValid { get; }
}