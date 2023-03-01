using Avalonia.Controls.Notifications;

namespace Zafiro.Avalonia;

public interface INotificationService
{
    void ShowMessage(string message);
}

public class NotificationService : INotificationService
{
    private readonly IManagedNotificationManager managedNotification;

    public NotificationService(IManagedNotificationManager managedNotification)
    {
        this.managedNotification = managedNotification;
    }

    public void ShowMessage(string message)
    {
        managedNotification.Show(message);
    }
}