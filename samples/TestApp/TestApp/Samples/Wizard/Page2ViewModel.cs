using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.Wizard;

internal class Page2ViewModel : ReactiveValidationObject, IValidatable
{
    public Page2ViewModel()
    {
        this.ValidationRule(x => x.Message, x => !string.IsNullOrWhiteSpace(x), "Can't be blank");
    }

    public IObservable<bool> IsValid => this.IsValid();
    [Reactive]
    public string? Message { get; set; }
}