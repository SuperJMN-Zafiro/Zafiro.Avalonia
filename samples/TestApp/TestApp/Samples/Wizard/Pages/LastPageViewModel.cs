﻿using System;
using System.Reactive.Linq;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace TestApp.Samples.Wizard.Pages;

public class LastPageViewModel : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid => Observable.Return(true);
}