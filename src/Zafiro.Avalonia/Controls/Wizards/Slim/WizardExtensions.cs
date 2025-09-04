using System.Reactive;
using System.Reactive.Disposables;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.ViewLocators;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public static class WizardExtensions
{
    public static async Task<Maybe<T>> Navigate<T>(this ISlimWizard<T> wizard, INavigator navigator)
    {
        var tcs = new TaskCompletionSource<Maybe<T>>();
        var disposable = new CompositeDisposable();

        await navigator.Go(() =>
        {
            wizard.Finished.SelectMany(async result =>
                {
                    var r = await navigator.GoBack().Map<Unit, T>(_ => result);
                    if (r.IsFailure)
                    {
                        throw new InvalidOperationException("Failed to navigate back from wizard.");
                    }

                    return r.Value;
                })
                .Take(1)
                .Do(result => tcs.SetResult(result))
                .Subscribe()
                .DisposeWith(disposable);

            return ApplicationUtils.ExecuteOnUIThread(() => CreateUserControl(wizard, navigator, tcs));
        });

        var navigateResult = await tcs.Task;
        disposable.Dispose();
        return navigateResult;
    }

    private static UserControl CreateUserControl<T>(ISlimWizard<T> wizard, INavigator navigator, TaskCompletionSource<Maybe<T>> tcs)
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
    }
}