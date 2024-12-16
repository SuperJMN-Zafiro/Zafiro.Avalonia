using System;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizards.Pages;

public partial class SecondPageViewModel : ReactiveValidationObject, IValidatable
{
    public int Number { get;  }
    
    public IObservable<bool> IsValid => this.IsValid();
    
    [Reactive] private bool isChecked;

    public SecondPageViewModel(int number)
    {
        Number = number;
        this.ValidationRule(x => x.IsChecked, b => b, "Is must be checked");
    }
}