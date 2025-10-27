using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Reactive;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Classic;

namespace Zafiro.Avalonia.Dialogs.Wizards.Classic;

public static class WizardExtensions
{
    public static Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, IWizard<TResult> wizard, string title)
    {
        return dialog.ShowAndGetResult(wizard, title, closeable => GetOptions(wizard, closeable), x => x.GetResult());
    }

    private static IEnumerable<IOption> GetOptions(IWizard wizard, ICloseable closeable)
    {
        var canCancel = wizard.IsBusy.CombineLatest(wizard.IsLastPage, (a, b) => !a && !b);
        var cancel = ReactiveCommand.Create(closeable.Dismiss, canCancel).Enhance();
        var close = ReactiveCommand.Create(closeable.Close, wizard.IsLastPage).Enhance();

        Settings settings = new Settings
        {
            IsDefault = true,
            IsVisible = wizard.IsLastPage.Not(),
        };
        Settings settings1 = new Settings
        {
            IsCancel = true,
            IsVisible = canCancel,
            Role = OptionRole.Cancel,
        };
        Settings settings2 = new Settings
        {
            IsDefault = true,
            IsVisible = wizard.IsLastPage
        };
        return
        [
            new Option("Next", wizard.Next, settings),
            new Option("Cancel", cancel, settings1),
            new Option("Close", close, settings2)
        ];
    }
}