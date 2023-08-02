using CSharpFunctionalExtensions;
using System.Reactive;
using System.Reactive.Linq;
using Zafiro.Core.Functional;
using Zafiro.UI;

namespace Zafiro.Avalonia.Mixins;

public static class GuiExtensions
{
    public static IDisposable HandleErrorsWith<T>(this IObservable<Result<T>> result, INotificationService notificationService)
    {
        return result.Failures().SelectMany(async error =>
        {
            await notificationService.Show(error);
            return Unit.Default;
        }).Subscribe();
    }

    public static IDisposable HandleErrorsWith(this IObservable<Result> result, INotificationService notificationService)
    {
        return result.Failures().SelectMany(async error =>
        {
            await notificationService.Show(error);
            return Unit.Default;
        }).Subscribe();
    }
}