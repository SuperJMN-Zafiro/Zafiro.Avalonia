using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.Validation.Helpers;
using TestApp.Samples.Wizard.Pages;
using Zafiro.Avalonia.NewWizard;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel : ReactiveValidationObject
{
    public WizardSampleViewModel()
    {
        Wizard = new Wizard<SecondPageViewModel>(new List<IPage<IValidatable, IValidatable>>()
        {
            new Zafiro.Avalonia.NewWizard.Page<IValidatable, IValidatable>(_ => new FirstPageViewModel()),
            new Zafiro.Avalonia.NewWizard.Page<IValidatable, IValidatable>(validatable => new SecondPageViewModel(((FirstPageViewModel)validatable).Number))
        });
        IsComplete = Observable.FromAsync(() => Wizard.Result);
    }

    public IObservable<SecondPageViewModel> IsComplete { get; }

    public IWizard<SecondPageViewModel, IPage<IValidatable, IValidatable>> Wizard { get; }
}