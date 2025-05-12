using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Dialogs.Wizards.Slim;

public static class WizardExtensions
{
    public static async Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, ISlimWizard<TResult> wizard, string title)
    {
        var disposables = new CompositeDisposable();

        var nextOption = new NextOption(wizard).DisposeWith(disposables);

        Func<ICloseable, IEnumerable<IOption>> optionsFactory = closeable =>
        {
            var canCancel = wizard.WhenAnyValue(slimWizard => slimWizard.CurrentPage).Select(x => x.Index != wizard.TotalPages - 1);
            var cancel = ReactiveCommand.Create(closeable.Dismiss, canCancel).Enhance();
            wizard.Finished.Subscribe(_ => closeable.Close()).DisposeWith(disposables);

            return
            [
                nextOption,
                OptionBuilder.Create("Cancel", cancel, new Settings
                {
                    IsCancel = true,
                    Role = OptionRole.Cancel,
                    IsVisible = canCancel,
                }),
            ];
        };

        var showAndGetResult = await dialog.ShowAndGetResult(wizard, title, optionsFactory, x => x.Finished.FirstAsync().ToTask());

        disposables.Dispose();

        return showAndGetResult;
    }
}