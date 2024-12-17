using System;
using System.Reactive.Linq;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew.Wizards.Pages;

public class ThirdPageViewModel : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid { get; } = Observable.Return(true);

    public ThirdPageViewModel()
    {
        
    }
}