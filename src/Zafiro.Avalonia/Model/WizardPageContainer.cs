using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Model;

public class WizardPageContainer : IWizardPage
{
    public IValidatable Page { get; }

    public WizardPageContainer(IValidatable page, Maybe<string> nextText)
    {
        Page = page;
        NextText = nextText;
    }

    public IObservable<bool> IsValid => Page.IsValid;
    public Maybe<string> NextText { get; }
}