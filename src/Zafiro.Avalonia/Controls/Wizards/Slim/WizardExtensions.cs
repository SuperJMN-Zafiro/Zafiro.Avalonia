using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Threading;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public static class WizardExtensions
{
    public static async Task<Maybe<T>> Navigate<T>(this ISlimWizard<T> wizard, INavigator navigator)
    {
        var tcs = new TaskCompletionSource<Maybe<T>>();
        var compositeDisposable = new CompositeDisposable();

        await navigator.Go(() =>
        {
            wizard.Finished.SelectMany(async result =>
                {
                    var r = await Dispatcher.UIThread.InvokeAsync(() => navigator.GoBack().Map<Unit, T>(_ => result));
                    if (r.IsFailure)
                    {
                        throw new InvalidOperationException("Failed to navigate back from wizard.");
                    }

                    return r.Value;
                })
                .Do(result => tcs.SetResult(result))
                .Subscribe()
                .DisposeWith(compositeDisposable);

            return Dispatcher.UIThread.Invoke(() =>
            {
                return new UserControl
                {
                    Content = new WizardNavigator
                    {
                        Wizard = wizard, Cancel = ReactiveCommand.CreateFromTask(async () =>
                        {
                            await navigator.GoBack();
                            tcs.SetResult(Maybe<T>.None);
                        }).Enhance()
                    }
                };
            });
        });


        var result = await tcs.Task;
        compositeDisposable.Dispose();
        return result;
    }
}