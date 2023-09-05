using System;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Model;

namespace TestApp.Samples.Wizard.Pages;

public class FirstPageViewModel : ReactiveValidationObject, IValidatable
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i == 2, "Must be even");
        this.IsValid().Subscribe(b => { });
    }

    [Reactive]
    public int Number { get; set; }

    public IObservable<bool> IsValid => this.IsValid();

    public Maybe<string> NextText { get; } = "Continue";
    public bool ShowNext { get; } = true;
}