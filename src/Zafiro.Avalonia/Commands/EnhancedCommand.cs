using System.Windows.Input;

namespace Zafiro.Avalonia.Commands;

public static class EnhancedCommand
{
    public static EnhancedCommandOf<T, Q> Create<T, Q>(ReactiveCommandBase<T, Q> reactiveCommand)
    {
        return new EnhancedCommandOf<T, Q>(reactiveCommand);
    }
    
    public class EnhancedCommandOf<T, Q> : IEnhancedCommandOf<T, Q>
    {
        private readonly ICommand command;
        private readonly ReactiveCommandBase<T, Q> reactiveCommand;

        public EnhancedCommandOf(ReactiveCommandBase<T, Q> reactiveCommandBase)
        {
            command = reactiveCommandBase;
            reactiveCommand = reactiveCommandBase;
        }

        public void Dispose()
        {
        }

        bool ICommand.CanExecute(object? parameter) => command.CanExecute(parameter);

        public void Execute(object? parameter) => command.Execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => command.CanExecuteChanged += value;
            remove => command.CanExecuteChanged -= value;
        }

        public IObservable<Exception> ThrownExceptions => reactiveCommand.ThrownExceptions;

        public IObservable<bool> IsExecuting => reactiveCommand.IsExecuting;

        public IObservable<bool> CanExecute => ((IReactiveCommand) command).CanExecute;

        public IDisposable Subscribe(IObserver<Q> observer) => reactiveCommand.Subscribe(observer);

        public IObservable<Q> Execute(T parameter) => reactiveCommand.Execute(parameter);

        public IObservable<Q> Execute() => reactiveCommand.Execute();
    }
}