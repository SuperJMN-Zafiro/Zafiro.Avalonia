using Avalonia.Controls.Notifications;
using Zafiro.Avalonia.Interfaces;

namespace Zafiro.Avalonia.Notifications;

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