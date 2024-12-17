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
        var cancel = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, canCancel));
        var close = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, wizard.IsLastPage));

        return
        [
            OptionBuilder.Create("Previous", wizard.Back, isVisible: ((IReactiveCommand)wizard.Back).CanExecute.CombineLatest(wizard.IsLastPage, (a, b) => a && !b)),
            OptionBuilder.Create("Next", wizard.Next, isVisible: ((IReactiveCommand)wizard.Next).CanExecute, isDefault: true),
            OptionBuilder.Create("Cancel", cancel, isVisible: canCancel, isCancel: true),
            OptionBuilder.Create("Close", close, isVisible: wizard.IsLastPage, isDefault: true)
        ];
    }
}