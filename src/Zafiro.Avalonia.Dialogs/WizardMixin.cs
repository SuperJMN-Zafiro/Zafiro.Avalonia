using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Dialogs;

public static class WizardMixin
{
    public static Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, IWizard<TResult> wizard, string title)
    {
        return dialog.ShowAndGetResult(wizard, title, x => x.IsLastPage, wizard.OptionsForCloseable, x => x.Result);
    }
    
    public static IEnumerable<IOption> OptionsForCloseable(this IWizard wizard, ICloseable closeable)
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