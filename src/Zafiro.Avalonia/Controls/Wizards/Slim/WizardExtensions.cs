using System.Reactive.Disposables;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Misc;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public static class WizardExtensions
{
    public static Task<Maybe<T>> Navigate<T>(this ISlimWizard<T> wizard, INavigator navigator)
    {
        return ExecuteWizardNavigation(wizard, navigator);
    }

    private static async Task<Maybe<T>> ExecuteWizardNavigation<T>(
        ISlimWizard<T> wizard,
        INavigator navigator)
    {
        var resultSource = new TaskCompletionSource<Maybe<T>>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        using var disposables = new CompositeDisposable();

        SetupWizardHandlers(wizard, navigator, resultSource, disposables);
        await navigator.Go(() => CreateWizardControl(wizard, navigator, resultSource, disposables));

        return await resultSource.Task.ConfigureAwait(false);
    }

    private static void SetupWizardHandlers<T>(
        ISlimWizard<T> wizard,
        INavigator navigator,
        TaskCompletionSource<Maybe<T>> resultSource,
        CompositeDisposable disposables)
    {
        wizard.Finished
            .SelectMany(result => HandleWizardCompletion(navigator, result))
            .Take(1) // Asegurar que solo se procese una vez
            .Subscribe(
                result => resultSource.TrySetResult(result),
                error => resultSource.TrySetResult(Maybe<T>.None))
            .DisposeWith(disposables);
    }

    private static async Task<Maybe<T>> HandleWizardCompletion<T>(INavigator navigator, T result)
    {
        try
        {
            var navResult = await ApplicationUtils.ExecuteOnUIThreadAsync(async () =>
            {
                var backResult = await navigator.GoBack();
                return backResult.Map(_ => result);
            });

            return navResult.AsMaybe();
        }
        catch
        {
            return Maybe<T>.None;
        }
    }

    private static UserControl CreateWizardControl<T>(
        ISlimWizard<T> wizard,
        INavigator navigator,
        TaskCompletionSource<Maybe<T>> resultSource,
        CompositeDisposable disposables)
    {
        return ApplicationUtils.ExecuteOnUIThread(() => CreateWizardControlCore(wizard, navigator, resultSource, disposables));
    }

    private static UserControl CreateWizardControlCore<T>(
        ISlimWizard<T> wizard,
        INavigator navigator,
        TaskCompletionSource<Maybe<T>> resultSource,
        CompositeDisposable disposables)
    {
        var cancelCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await navigator.GoBack().ConfigureAwait(false);
            resultSource.TrySetResult(Maybe<T>.None);
        }).Enhance();

        // Asegurar que el comando se dispose correctamente
        cancelCommand.DisposeWith(disposables);

        return new UserControl
        {
            Content = new WizardNavigator
            {
                Wizard = wizard,
                Cancel = cancelCommand
            }
        };
    }
}