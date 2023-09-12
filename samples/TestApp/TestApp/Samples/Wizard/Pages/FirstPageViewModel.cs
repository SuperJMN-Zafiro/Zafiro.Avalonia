using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace TestApp.Samples.Wizard.Pages;

public class FirstPageViewModel : ReactiveValidationObject, IValidatable
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i % 2 == 0, "Must be even");
    }

    [Reactive]
    public int Number { get; set; }

    public IObservable<bool> IsValid => this.IsValid();
}