using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Zafiro.Avalonia.Model;

public class Wizard : ReactiveObject, IWizard
{
    public Wizard(IList<IWizardPage> pages)
    {
        Pages = pages;

        ActivePage = this.WhenAnyValue(x => x.CurrentPageIndex).Select(i => Pages[i]);
        var isCurrentPageValid = ActivePage.Select(x => x.IsValid).Switch();

        var canGoNext = this
            .WhenAnyValue(x => x.CurrentPageIndex, x => x == Pages.Count - 1)
            .CombineLatest(isCurrentPageValid, (isLastPage, isValid) => isValid && !isLastPage);

        var canGoBack = this.WhenAnyValue(x => x.CurrentPageIndex, x => x > 0);

        GoNextCommand = ReactiveCommand.Create(() => CurrentPageIndex++, canGoNext);
        BackCommand = ReactiveCommand.Create(() => CurrentPageIndex--, canGoBack);
        CanGoNext = canGoNext;
    }

    [Reactive] public int CurrentPageIndex { get; set; }

    public IObservable<bool> CanGoNext { get; }

    public IObservable<IWizardPage> ActivePage { get; }

    public IReactiveCommand BackCommand { get; set; }

    public ICommand GoNextCommand { get; set; }

    public IList<IWizardPage> Pages { get; }
}