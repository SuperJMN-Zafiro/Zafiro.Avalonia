using Avalonia.Controls.Notifications;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Services;

[PublicAPI]
public class NotificationService : INotificationService
{
    public static INotificationService Instance = GetNotificationService();
    private readonly IManagedNotificationManager managedNotification;

    public NotificationService(IManagedNotificationManager managedNotification)
    {
        this.managedNotification = managedNotification;
    }

    public Task Show(string message, Maybe<string> title)
    {
        managedNotification.Show(new Notification(title.GetValueOrDefault(), message));
        return Task.CompletedTask;
    }

    private static INotificationService GetNotificationService()
    {
        if (Design.IsDesignMode)
        {
            return new DummyNotificationService();
        }

        return new NotificationService(new WindowNotificationManager(ApplicationUtils.TopLevel().GetValueOrThrow("Cannot get Top Level for the Notification Service"))
        {
            Position = NotificationPosition.BottomRight,
        });
    }
}