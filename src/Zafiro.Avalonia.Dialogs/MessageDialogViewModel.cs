using System.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

public class MessageDialogViewModel(string message)
{
    public string Message { get; } = message;
}