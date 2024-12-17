using System.Reactive;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public static class OptionBuilder
{
    public static Option<T, Q> Create<T, Q>(string title, IEnhancedCommand<T, Q> command, bool isDefault = false, bool isCancel = false)
    {
        return new Option<T, Q>(title, command, isDefault, isCancel);
    }
    
    public static Option<Unit, Unit> Create<T, Q>(string title, IEnhancedCommand<Unit, Unit> command, bool isDefault = false, bool isCancel = false)
    {
        return new Option<Unit, Unit>(title, command, isDefault, isCancel);
    }

    public static Option Create(string title, IEnhancedCommand command, bool isDefault = false, bool isCancel = false)
    {
        return new Option(title, command, isDefault, isCancel);
    }
}