using Avalonia.Controls.Notifications;
using JetBrains.Annotations;
using Zafiro.UI;

namespace Zafiro.Avalonia.Notifications;

[PublicAPI]
public class NotificationService : INotificationService
{
    private readonly IManagedNotificationManager managedNotification;

    public NotificationService(IManagedNotificationManager managedNotification)
    {
        this.managedNotification = managedNotification;
    }

    public Task Show(string message)
    {
        managedNotification.Show(message);
        return Task.CompletedTask;
    }
}