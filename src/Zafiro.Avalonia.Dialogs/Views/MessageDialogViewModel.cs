namespace Zafiro.Avalonia.Dialogs.Views;

public class MessageDialogViewModel(string message)
{
    public string Message { get; } = message;
}