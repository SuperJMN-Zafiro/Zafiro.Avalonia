using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI.Wizards.Classic.Builder;

namespace TestApp.Samples.Wizard.Pages;

public class ThirdPageViewModel(SecondPageViewModel second) : ReactiveValidationObject, IStep
{
    public string SomeProperty => "Hello!";
    public IObservable<bool> IsValid { get; } = Observable.Return(true);

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;

    public Maybe<string> Title => "Final page";
}