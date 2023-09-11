namespace Zafiro.Avalonia.Wizard.Interfaces;

public interface IMyCommand
{
    Task Execute();
    IObservable<bool> CanExecute { get; }
}