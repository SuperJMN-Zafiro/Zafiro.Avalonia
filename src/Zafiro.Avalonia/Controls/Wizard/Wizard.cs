using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Zafiro.Avalonia.Controls.Wizard;

public partial class Wizard : ReactiveObject, IWizard
{
    [Reactive]
    public int CurrentPageIndex { get; set; }

    public Wizard(IList<IWizardPage> pages)
    {
        Pages = pages;

        ActivePage = this.WhenAnyValue(x => x.CurrentPageIndex).Select(i => Pages[i]);
        var currentPageIsValid = ActivePage.Select(x => x.IsValid).Switch();

        var canGoNext = this.WhenAnyValue(x => x.CurrentPageIndex, x => x < Pages.Count - 1).CombineLatest(currentPageIsValid, (inRange, isValid) => inRange && isValid);
        var canBack = this.WhenAnyValue(x => x.CurrentPageIndex, x => x > 0);

        GoNextCommand = ReactiveCommand.Create(() => CurrentPageIndex++, canGoNext);
        BackCommand = ReactiveCommand.Create(() => CurrentPageIndex--, canBack);
        CanGoNext = canGoNext;
    }

    public IObservable<bool> CanGoNext { get; }

    public IObservable<IWizardPage> ActivePage { get; }

    public IReactiveCommand BackCommand { get; set; }

    public ICommand GoNextCommand { get; set; }

    public IList<IWizardPage> Pages { get; }
}