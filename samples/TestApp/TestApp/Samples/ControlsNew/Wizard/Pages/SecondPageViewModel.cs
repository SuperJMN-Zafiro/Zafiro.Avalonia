using System;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public partial class SecondPageViewModel : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid => this.IsValid();
    
    [Reactive] private bool isChecked;

    public SecondPageViewModel()
    {
        this.ValidationRule<SecondPageViewModel, bool>(x => x.IsChecked, b => b, "Is must be checked");
    }
}