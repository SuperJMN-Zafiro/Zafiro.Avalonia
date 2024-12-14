namespace Zafiro.Avalonia.Commands;

public interface IEnhancedCommandOf<T, Q> : IReactiveCommand<T, Q>,
    IEnhancedCommand;