using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards;

namespace Zafiro.Avalonia.Dialogs;

public static class Wizard
{
    public static IEnumerable<IOption> OptionsForCloseable(this IWizard wizard, ICloseable closeable)
    {
        var canCancel = wizard.IsBusy.CombineLatest(wizard.IsLastPage, (a, b) => !a && !b);
        var cancel = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss, canCancel));
        var close = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, wizard.IsLastPage));

        return
        [
            OptionBuilder.Create("Next", wizard.Next, new Settings(isVisible: ((IReactiveCommand)wizard.Next).CanExecute, isDefault: true)),
            OptionBuilder.Create("Cancel", cancel, new Settings(isVisible: canCancel, isCancel: true)
            {
                Role = OptionRole.Cancel,
            }),
            OptionBuilder.Create("Close", close, new Settings(isVisible: wizard.IsLastPage, isDefault: true))
        ];
    }
}