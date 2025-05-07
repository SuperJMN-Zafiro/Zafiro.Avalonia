using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizard;

namespace Zafiro.Avalonia.Dialogs;

public static class SlimWizardMixin
{
    public static async Task<Maybe<TResult>> ShowWizard<TResult>(this IDialog dialog, IWizard<TResult> wizard, string title)
    {
        var disposables = new CompositeDisposable();

        var nextOption = new NextOption(wizard).DisposeWith(disposables);

        Func<ICloseable, IEnumerable<IOption>> optionsFactory = closeable =>
        {
            var cancel = EnhancedCommand.Create(ReactiveCommand.Create(closeable.Dismiss));
            wizard.FinishedOfT.Subscribe(_ => closeable.Close()).DisposeWith(disposables);

            return
            [
                nextOption,
                OptionBuilder.Create("Cancel", cancel, new Settings
                {
                    IsCancel = true,
                    Role = OptionRole.Cancel,
                }),
            ];
        };

        var showAndGetResult = await dialog.ShowAndGetResult(wizard, title, optionsFactory, x => x.FinishedOfT.FirstAsync().ToTask());

        disposables.Dispose();

        return showAndGetResult;
    }
}