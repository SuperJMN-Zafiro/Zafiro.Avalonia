using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace TestApp.Samples.Wizard.Pages;

public class FirstPageViewModel : ReactiveValidationObject, IValidatable, IViewModel
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i % 2 == 0, "Must be even");
    }

    [Reactive]
    public int Number { get; set; }

    public IObservable<bool> IsValid => this.IsValid();
}