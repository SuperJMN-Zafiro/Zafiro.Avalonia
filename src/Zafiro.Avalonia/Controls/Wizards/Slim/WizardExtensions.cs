using System.Reactive;
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

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
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
                    .Do(result => tcs.SetResult(result))
                    .Subscribe();

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

        return await tcs.Task;
    }
}