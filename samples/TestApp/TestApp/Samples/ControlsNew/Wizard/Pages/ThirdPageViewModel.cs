using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public class ThirdPageViewModel(SecondPageViewModel second) : ReactiveValidationObject, IStep
{
    public IObservable<bool> IsValid { get; } = Observable.Return(true);

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;

    public Maybe<string> Title => "Final page";
}