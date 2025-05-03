using System;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI.Wizard;

namespace TestApp.Samples.ControlsNew.SlimWizard.Pages;

public partial class Page1ViewModel : ReactiveValidationObject, ITitled
{
    [Reactive] private int? number;

    public Page1ViewModel()
    {
        this.ValidationRule(x => x.Number, i => i % 2 == 0, "Number must be even");
    }

    public IObservable<bool> IsValid => this.IsValid();
    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public string Title => "First page";
}