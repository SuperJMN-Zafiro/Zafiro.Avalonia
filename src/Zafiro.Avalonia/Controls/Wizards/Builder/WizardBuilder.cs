namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public static class WizardBuilder
{
    public static WizardBuilder<TOutput> StartWith<TOutput>(Func<TOutput> factory)
        where TOutput : IValidatable
    {
        var pages = new List<PageFactory>();
        pages.Add(new PageFactory(() => factory()));
        return new WizardBuilder<TOutput>(pages);
    }
}

public class WizardBuilder<TPrevious> where TPrevious : IValidatable
{
    private readonly List<PageFactory> pages;

    internal WizardBuilder(List<PageFactory> pages)
    {
        this.pages = pages;
    }

    public WizardBuilder<TNext> Then<TNext>(Func<TPrevious, TNext> factory)
        where TNext : IValidatable
    {
        var previousPageIndex = pages.Count - 1;
        
        pages.Add(new PageFactory(() => {
            var previousPage = (TPrevious)pages[previousPageIndex].GetInstance();
            return factory(previousPage);
        }));
        
        return new WizardBuilder<TNext>(pages);
    }

    public IList<Func<IValidatable>> Build()
    {
        return pages.Select(p => new Func<IValidatable>(() => p.GetInstance())).ToList();
    }
}