using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using TestApp.Samples.Wizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.NewWizard;
using Zafiro.Avalonia.NewWizard.Interfaces;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel : ReactiveValidationObject, IViewModel
{
    public WizardSampleViewModel(IDialogService dialogService, INotificationService notificationService)
    {
        ShowWizard = ReactiveCommand.CreateFromTask(() => dialogService.ShowDialog(CreateWizard(), "Wizardo de los buenos", x => Observable.FromAsync(() => x.Result)));
        ShowWizard
            .Values()
            .SelectMany(x => TaskMixin.ToSignal(() => notificationService.Show($"You finished the wizard and selected number {x.SelectedNumber}", "Yay")))
            .Subscribe();

        ShowWizard
            .Empties()
            .SelectMany(_ => TaskMixin.ToSignal(() => notificationService.Show($"You dismissed the wizard", "Dude...!")))
            .Subscribe();
    }

    private static Wizard<SecondPageViewModel> CreateWizard()
    {
        return new Wizard<SecondPageViewModel>(new List<IPage<IValidatable, IValidatable>>()
        {
            new Page<IValidatable, IValidatable>(_ => new FirstPageViewModel(), "Next"),
            new Page<IValidatable, IValidatable>(validatable => new SecondPageViewModel(((FirstPageViewModel)validatable).Number), "Finish")
        });
    }

    public ReactiveCommand<Unit, Maybe<SecondPageViewModel>> ShowWizard { get; set; }

    public IObservable<SecondPageViewModel> IsComplete { get; }

    public IWizard<SecondPageViewModel, IPage<IValidatable, IValidatable>> Wizard { get; }
}