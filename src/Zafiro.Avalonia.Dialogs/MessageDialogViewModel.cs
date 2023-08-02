namespace Zafiro.Avalonia.Dialogs;

public class MessageDialogViewModel
{
    public string Message { get; }

    public MessageDialogViewModel(string message)
    {
        Message = message;
    }
}