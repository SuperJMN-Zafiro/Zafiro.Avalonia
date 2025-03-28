using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Controls.Wizards;
using Zafiro.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Dialogs;

public static class WizardMixin
{
    public static Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, IWizard<TResult> wizard, string title)
    {
        return dialog.ShowAndGetResult(wizard, title, closeable => GetOptions(wizard, closeable), x => x.GetResult());
    }

    private static IEnumerable<IOption> GetOptions(IWizard wizard, ICloseable closeable)
    {
        var canCancel = wizard.IsBusy.CombineLatest(wizard.IsLastPage, (a, b) => !a && !b);
        var cancel = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss, canCancel));
        var close = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, wizard.IsLastPage));

        return
        [
            OptionBuilder.Create("Next", wizard.Next, new Settings
            {
                IsDefault = true,
                IsVisible = wizard.IsLastPage.Not(),
            }),
            OptionBuilder.Create("Cancel", cancel, new Settings
            {
                IsCancel = true,
                IsVisible = canCancel,
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("Close", close, new Settings
            {
                IsDefault = true,
                IsVisible = wizard.IsLastPage
            })
        ];
    }
}