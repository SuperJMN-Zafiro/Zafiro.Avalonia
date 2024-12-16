using System;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public partial class FirstPageViewModel : ReactiveValidationObject, IValidatable
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i  % 2 == 0, "Number must be even" );
    }

    [Reactive] private int? number;
    
    public IObservable<bool> IsValid => this.IsValid();
}