using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI.Wizards.Classic.Builder;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public partial class SecondPageViewModel : ReactiveValidationObject, IStep
{
    [Reactive] private bool isChecked;

    [Reactive] private string? text;

    public SecondPageViewModel(int number)
    {
        Number = number;
        this.ValidationRule(x => x.IsChecked, b => b, "Is must be checked");
    }

    public int Number { get; }

    public IObservable<bool> IsValid => this.IsValid();

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public Maybe<string> Title => "Second Page";
}