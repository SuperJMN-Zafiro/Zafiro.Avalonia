using System.Reactive.Disposables;
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
        using var disposables = new CompositeDisposable();

        await navigator.Go(() =>
        {
            var cancel = ReactiveCommand.CreateFromTask(async () =>
            {
                await navigator.GoBack();
                tcs.TrySetResult(Maybe<T>.None);
            }).Enhance();
            cancel.DisposeWith(disposables);

            wizard.Finished
                .SelectMany(result =>
                    Dispatcher.UIThread.InvokeAsync(() =>
                        navigator.GoBack().Map(_ => result)))
                .Subscribe(r =>
                {
                    if (r.IsFailure)
                    {
                        tcs.TrySetException(new InvalidOperationException("Failed to navigate back from wizard."));
                        return;
                    }

                    tcs.TrySetResult(Maybe.From(r.Value));
                }, ex => tcs.TrySetException(ex))
                .DisposeWith(disposables);

            return Dispatcher.UIThread.Invoke(() => new UserControl
            {
                Content = new WizardNavigator
                {
                    Wizard = wizard,
                    Cancel = cancel
                }
            });
        });

        return await tcs.Task;
    }
}
