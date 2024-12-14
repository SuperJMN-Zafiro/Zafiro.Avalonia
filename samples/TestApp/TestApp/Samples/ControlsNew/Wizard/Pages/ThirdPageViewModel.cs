using System;
using System.Reactive.Linq;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizard.Pages;

public class ThirdPageViewModel : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid { get; } = Observable.Return(true);
}