using System.Reactive;
using System.Windows.Input;

namespace Zafiro.Avalonia.Commands;

public interface IEnhancedCommand : 
    ICommand,
    IReactiveCommand;

public interface IEnhancedUnitCommand : IReactiveCommand<Unit, Unit>;

public interface IEnhancedCommand<T, Q> : IReactiveCommand<T, Q>,
    IEnhancedCommand;