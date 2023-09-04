using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizard;

namespace TestApp.Samples.Wizard.Pages;

public class FirstPageViewModel : ReactiveValidationObject, IWizardPage
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i == 2, "Must be even");
        this.IsValid().Subscribe(b => { });
    }

    [Reactive]
    public int Number { get; set; }

    public IObservable<bool> IsValid => this.IsValid();

    public string NextText { get; } = "Continue";
    public bool ShowNext { get; } = true;
}