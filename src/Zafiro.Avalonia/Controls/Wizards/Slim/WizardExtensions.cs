using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public static class WizardExtensions
{
    public static async Task<Maybe<T>> Navigate<T>(this ISlimWizard<T> wizard, INavigator navigator)
    {
        var tcs = new TaskCompletionSource<Maybe<T>>(TaskCreationOptions.RunContinuationsAsynchronously);

        var cancel = ReactiveCommand.CreateFromTask(async () =>
        {
            var back = await navigator.GoBack();
            if (back.IsFailure)
            {
                throw new InvalidOperationException("Failed to navigate back from wizard.");
            }

            tcs.TrySetResult(Maybe<T>.None);
        });

        var subscription = wizard.Finished
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async result =>
            {
                var back = await navigator.GoBack();
                if (back.IsFailure)
                {
                    throw new InvalidOperationException("Failed to navigate back from wizard.");
                }

                tcs.TrySetResult(Maybe.From(result));
            }, ex => tcs.TrySetException(ex));

        _ = tcs.Task.ContinueWith(_ =>
        {
            subscription.Dispose();
            cancel.Dispose();
        }, TaskScheduler.Default);

        await Dispatcher.UIThread.InvokeAsync(() => navigator.Go(() => new UserControl
        {
            Content = new WizardNavigator
            {
                Wizard = wizard,
                Cancel = cancel.Enhance()
            }
        }));

        return await tcs.Task;
    }
}
