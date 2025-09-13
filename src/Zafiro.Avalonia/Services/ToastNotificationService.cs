using Avalonia.Layout;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Ursa.Controls;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.Services;

[PublicAPI]
public class ToastNotificationService : INotificationService
{
    public static INotificationService Instance = GetNotificationService();
    readonly IToastManager toastManager;

    public ToastNotificationService(IToastManager toastManager)
    {
        this.toastManager = toastManager;
    }

    public Task Show(string message, Maybe<string> title)
    {
        Action action = () =>
        {
            var content = title.Map(t => $"{t}: {message}").GetValueOrDefault(message);
            toastManager.Show(new Toast(content));
        };
        action.ExecuteOnUIThread();
        return Task.CompletedTask;
    }

    static INotificationService GetNotificationService()
    {
        if (Design.IsDesignMode)
        {
            return new DummyNotificationService();
        }

        var topLevel = ApplicationUtils.TopLevel().GetValueOrThrow("Cannot get Top Level for the Notification Service");
        return new ToastNotificationService(new WindowToastManager(topLevel)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        });
    }
}
