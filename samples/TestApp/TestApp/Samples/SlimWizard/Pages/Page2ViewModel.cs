using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.SlimWizard.Pages;

public partial class Page2ViewModel : ReactiveValidationObject, IValidatable
{
    [Reactive] private bool isChecked;

    [Reactive] private string? text;

    public Page2ViewModel(int number)
    {
        Number = number;
        this.ValidationRule<Page2ViewModel, bool>(x => x.IsChecked, b => b, "Is must be checked");
    }

    public int Number { get; }

    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public Maybe<string> Title => "Second Page";

    public IObservable<bool> IsValid => this.IsValid();
}