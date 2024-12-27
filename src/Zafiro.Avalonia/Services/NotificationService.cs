using Avalonia.Controls.Notifications;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Services;

[PublicAPI]
public class NotificationService : INotificationService
{
    private readonly IManagedNotificationManager managedNotification;

    public NotificationService(IManagedNotificationManager managedNotification)
    {
        this.managedNotification = managedNotification;
    }

    public static INotificationService Instance = GetNotificationService();

    private static INotificationService GetNotificationService()
    {
        if (Design.IsDesignMode)
        {
            return new DummyNotificationService();
        }

        return new NotificationService(new WindowNotificationManager(Application.Current!.TopLevel().Value)
        {
            Position = NotificationPosition.BottomCenter,
        });
    }

    public Task Show(string message, Maybe<string> title)
    {
        managedNotification.Show(new Notification(title.GetValueOrDefault(), message));
        return Task.CompletedTask;
    }
}