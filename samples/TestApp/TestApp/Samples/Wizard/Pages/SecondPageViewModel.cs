using System;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Model;

namespace TestApp.Samples.Wizard.Pages;

public class SecondPageViewModel : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid => ((ReactiveValidationObject)this).IsValid();
    public Maybe<string> NextText { get; } = "";
    public bool ShowNext { get; } = false;
    
    [Reactive]
    public int Number { get; set; }
}