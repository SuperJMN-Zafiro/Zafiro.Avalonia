namespace Zafiro.Avalonia.WizardOld.Interfaces;

public interface IMyCommand
{
    Task Execute();
    IObservable<bool> CanExecute { get; }
}