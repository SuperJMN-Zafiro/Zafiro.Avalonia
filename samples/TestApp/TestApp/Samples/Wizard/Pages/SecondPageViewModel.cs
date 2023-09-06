using System;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Model;

namespace TestApp.Samples.Wizard.Pages;

public class SecondPageViewModel : ReactiveValidationObject, IValidatable
{
    public int SelectedNumber { get; }

    public SecondPageViewModel(int selectedNumber)
    {
        SelectedNumber = selectedNumber;
    }

    public IObservable<bool> IsValid => ((ReactiveValidationObject)this).IsValid();
}