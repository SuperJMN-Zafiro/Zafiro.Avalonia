using System;
using System.Reactive.Linq;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace TestApp.Samples.Wizard.Pages;

public class LastPageViewModel : ReactiveValidationObject, IValidatable, IViewModel
{
    public IObservable<bool> IsValid => Observable.Return(true);
}