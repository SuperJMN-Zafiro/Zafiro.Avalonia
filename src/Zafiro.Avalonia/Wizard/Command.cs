using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace Zafiro.Avalonia.Wizard;

internal class Command<TIn, TOut> : IMyCommand
{
    private readonly ReactiveCommand<TIn, TOut> command;

    public Command(ReactiveCommand<TIn, TOut> command)
    {
        this.command = command;
    }

    public async Task Execute()
    {
        await command.Execute();
    }

    public IObservable<bool> CanExecute => command.CanExecute;
}