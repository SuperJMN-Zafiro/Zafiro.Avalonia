namespace Zafiro.Avalonia.NewWizard.Interfaces;

public interface IMyCommand
{
    Task Execute();
    IObservable<bool> CanExecute { get; }
}