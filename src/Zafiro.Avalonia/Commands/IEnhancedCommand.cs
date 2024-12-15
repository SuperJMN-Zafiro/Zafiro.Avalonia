using System.Windows.Input;

namespace Zafiro.Avalonia.Commands;

public interface IEnhancedCommand : 
    ICommand,
    IReactiveCommand;