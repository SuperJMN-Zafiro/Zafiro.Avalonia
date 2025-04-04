namespace Zafiro.Avalonia.Controls.StatusBar.NotificationTypes;

public class MessageWithPathContent
{
    public MessageWithPathContent(string message, string pathResourceName)
    {
        Message = message;
        PathResourceName = pathResourceName;
    }

    public string Message { get; }
    public string PathResourceName { get; }
    public Valence Valence { get; set; } = Valence.Neutral;
}