using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizard.Pages;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizard;

using FirstPageViewModel = FirstPageViewModel;
using SecondPageViewModel = SecondPageViewModel;

public class WizardViewModel : ReactiveObject
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(() =>
        {
            var wizard = WizardBuilder
                .StartWith(() => new FirstPageViewModel())
                .Then(first => new SecondPageViewModel(first.Number!.Value))
                .Then(second => new ThirdPageViewModel(second))
                .FinishWith(third => "Hello!");
            
            return dialog.ShowWizard(wizard, "Such a nice wizard this is!");
        });

        LaunchWizard.Values()
            .SelectMany(x => Observable.FromAsync(() => dialog.ShowMessage("Wizard finished", $"Wizard result='{x}'")))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Maybe<string>> LaunchWizard { get; set; }
}