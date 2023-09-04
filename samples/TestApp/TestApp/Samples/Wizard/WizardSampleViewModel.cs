using System.Collections.Generic;
using ReactiveUI.Validation.Helpers;
using TestApp.Samples.Wizard.Pages;
using Zafiro.Avalonia.Controls.Wizard;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel : ReactiveValidationObject
{
    public List<IWizardPage> Pages { get; } = new()
    {
        new FirstPageViewModel(),
        new SecondPageViewModel(),
    };
}