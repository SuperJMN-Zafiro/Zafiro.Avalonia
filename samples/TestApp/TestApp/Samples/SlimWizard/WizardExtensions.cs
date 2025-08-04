using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Controls.Wizards.Slim;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Wizards.Slim;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Wizards.Slim;

namespace TestApp.Samples.SlimWizard;

public static class WizardExtensions
{
    public static async Task<Maybe<T>> Navigate<T>(this ISlimWizard<T> wizard, INavigator navigator)
    {
        var tcs = new TaskCompletionSource<Maybe<T>>();

        await navigator.Go(() =>
        {
            wizard.Finished.SelectMany(async result =>
                {
                    var r = await AsyncResultExtensionsLeftOperand.Map<Unit, T>(navigator.GoBack(), _ => result);
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

        return await tcs.Task;
    }

    public static Task<Maybe<T>> ShowDialog<T>(this ISlimWizard<T> wizard, IDialog navigator, string title)
    {
        return navigator.ShowWizard(wizard, title);
    }
}