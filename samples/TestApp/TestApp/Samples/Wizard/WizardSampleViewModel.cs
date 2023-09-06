using CSharpFunctionalExtensions;
using ReactiveUI.Validation.Helpers;
using TestApp.Samples.Wizard.Pages;
using Zafiro.Avalonia.Model;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel : ReactiveValidationObject
{
    public WizardSampleViewModel()
    {
        var pages = PageBuilder
            .PageFor(() => new FirstPageViewModel(), "Continue")
            .WithNext(first => new SecondPageViewModel(first.Number), "Oh sí")
            .WithNext(_ => new LastPageViewModel(), Maybe<string>.None)
            .Build();

        Wizard = new Zafiro.Avalonia.Model.Wizard(pages);
    }

    public IWizard Wizard { get; }
}