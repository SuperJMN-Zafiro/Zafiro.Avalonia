namespace Zafiro.Avalonia.Controls.StatusBar.NotificationTypes;

public class LoadingMessage
{
    public LoadingMessage(string message)
    {
        Message = message;
    }

    public string Message { get; }
}