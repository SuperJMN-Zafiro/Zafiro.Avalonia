using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizard;

namespace TestApp.Samples.Wizard.Pages;

public class SecondPageViewModel : ReactiveValidationObject, IWizardPage
{
    public IObservable<bool> IsValid => ((ReactiveValidationObject)this).IsValid();
    public string NextText { get; } = "";
    public bool ShowNext { get; } = false;
    
    [Reactive]
    public int Number { get; set; }
}