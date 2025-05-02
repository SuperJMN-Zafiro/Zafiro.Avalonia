using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace TestApp.Samples.ControlsNew.SlimWizard.Pages;

public partial class Page1ViewModel : ReactiveValidationObject, IStep
{
    [Reactive] private int? number;

    public Page1ViewModel()
    {
        this.ValidationRule(x => x.Number, i => i % 2 == 0, "Number must be even");
    }

    public IObservable<bool> IsValid => this.IsValid();
    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public Maybe<string> Title => "First page";
}