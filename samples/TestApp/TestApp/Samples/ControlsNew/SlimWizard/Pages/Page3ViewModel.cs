using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace TestApp.Samples.ControlsNew.SlimWizard.Pages;

public class Page3ViewModel(Page2ViewModel two) : ReactiveValidationObject, IStep
{
    public string SomeProperty => "Hello!";
    public IObservable<bool> IsValid { get; } = Observable.Return(true);

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;

    public Maybe<string> Title => "Final page";
}