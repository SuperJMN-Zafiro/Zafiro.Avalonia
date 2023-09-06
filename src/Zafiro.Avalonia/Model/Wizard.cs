using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Zafiro.Avalonia.Model;

public class Wizard : ReactiveObject, IWizard
{
    private readonly ObservableAsPropertyHelper<IPage> currentPage;
    private readonly IList<IPage> pages;

    public Wizard(IList<IPage> pages)
    {
        this.pages = pages;

        var currentPageObs = this.WhenAnyValue(wizard => wizard.CurrentPageIndex)
            .Do(CreateContent)
            .Select(i => pages[i]);
        currentPage = currentPageObs
            .ToProperty(this, wizard => wizard.CurrentPage);
        
        var isCurrentPageValid = this.WhenAnyValue(x => x.CurrentPage.Content!.IsValid).Switch();
        var canGoNext = this
            .WhenAnyValue(x => x.CurrentPageIndex, x => x == pages.Count - 1)
            .CombineLatest(isCurrentPageValid, (isLastPage, isValid) => isValid && !isLastPage);

        GoNext = ReactiveCommand.Create(() => CurrentPageIndex++, canGoNext);

        var canGoBack = this.WhenAnyValue(x => x.CurrentPageIndex, x => x > 0);

        GoBack = ReactiveCommand.Create(() => CurrentPageIndex--, canGoBack);
        CanGoNext = canGoNext;
    }

    public IReactiveCommand GoBack { get; set; }

    public IPage CurrentPage => currentPage.Value;

    public IEnumerable<IPage> Pages => pages.AsReadOnly();
    
    public ICommand GoNext { get; }

    [Reactive] public int CurrentPageIndex { get; private set; }

    public IObservable<bool> CanGoNext { get; }

    private void CreateContent(int n)
    {
        if (n > 0)
        {
            pages[n].CreateContent(pages[n - 1].Content);
        }
        else
        {
            if (pages[0].Content is null)
            {
                pages[0].CreateContent(null);
            }
        }
    }
}