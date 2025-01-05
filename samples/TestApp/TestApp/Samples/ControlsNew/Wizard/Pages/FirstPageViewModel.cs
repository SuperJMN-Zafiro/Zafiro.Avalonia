using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public partial class FirstPageViewModel : ReactiveValidationObject, IStep
{
    public FirstPageViewModel()
    {
        this.ValidationRule(x => x.Number, i => i  % 2 == 0, "Number must be even" );
    }

    [Reactive] private int? number;
    
    public IObservable<bool> IsValid => this.IsValid();
    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public Maybe<string> Title => Maybe<string>.None;
}