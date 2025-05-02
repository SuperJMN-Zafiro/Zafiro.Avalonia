using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace TestApp.Samples.ControlsNew.SlimWizard.Pages;

public partial class Page2ViewModel : ReactiveValidationObject, IStep
{
    [Reactive] private bool isChecked;

    [Reactive] private string? text;

    public Page2ViewModel(int number)
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