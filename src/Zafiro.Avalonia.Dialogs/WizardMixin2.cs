using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizard;

namespace Zafiro.Avalonia.Dialogs;

public static class WizardMixin2
{
    public static async Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, Wizard<TResult> wizard, string title)
    {
        return await dialog.ShowAndGetResult(wizard, title, closeable => GetOptions(wizard, closeable), x => x.Finished.FirstAsync().ToTask());
    }

    private static IEnumerable<IOption> GetOptions<TResult>(Wizard<TResult> wizard, ICloseable closeable)
    {
        //var canCancel = wizard.IsBusy.CombineLatest(wizard.IsLastPage, (a, b) => !a && !b);
        var cancel = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss));
        var close = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close));
        wizard.Finished.Subscribe(result => closeable.Close());

        return
        [
            OptionBuilder.Create("Back", EnhancedCommand.Create(wizard.BackCommand), new Settings
            {
                Role = OptionRole.Secondary,
            }),
            OptionBuilder.Create("Next", EnhancedCommand.Create(wizard.NextCommand), new Settings
            {
                IsDefault = true,
            }),
            OptionBuilder.Create("Cancel", cancel, new Settings
            {
                IsCancel = true,
                Role = OptionRole.Cancel,
            }),
            // OptionBuilder.Create("Close", close, new Settings
            // {
            //     IsDefault = true,
            // })
        ];
    }
}