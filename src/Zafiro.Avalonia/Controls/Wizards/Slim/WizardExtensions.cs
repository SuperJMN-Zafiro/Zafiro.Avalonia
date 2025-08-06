using System.Reactive;
using System.Reactive.Threading.Tasks;
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
        var cancelCommand = ReactiveCommand.CreateFromTask(async () => await navigator.GoBack());

        var finished = wizard.Finished.SelectMany(async result =>
        {
            var back = await navigator.GoBack().Map(_ => Maybe.From(result));
            if (back.IsFailure)
            {
                throw new InvalidOperationException("Failed to navigate back from wizard.");
            }

            return back.Value;
        });

        var cancellation = cancelCommand.Select(_ => Maybe<T>.None);

        var completion = finished.Merge(cancellation).FirstAsync().ToTask();

        await Dispatcher.UIThread.InvokeAsync(async () =>
            await navigator.Go(() => new UserControl
            {
                Content = new WizardNavigator
                {
                    Wizard = wizard,
                    Cancel = cancelCommand.Enhance()
                }
            }));

        return await completion;
    }
}
