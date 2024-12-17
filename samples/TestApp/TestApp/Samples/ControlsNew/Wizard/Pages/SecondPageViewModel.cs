using System;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public partial class SecondPageViewModel : ReactiveValidationObject, IStep
{
    public int Number { get;  }
    
    public IObservable<bool> IsValid => this.IsValid();
    
    [Reactive] private bool isChecked;

    public SecondPageViewModel(int number)
    {
        Number = number;
        this.ValidationRule(x => x.IsChecked, b => b, "Is must be checked");
    }

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
}