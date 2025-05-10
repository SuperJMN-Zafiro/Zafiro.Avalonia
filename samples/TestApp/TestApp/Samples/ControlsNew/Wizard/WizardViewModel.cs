using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Wizards.Classic;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI.Wizards.Classic.Builder;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizard;

public class WizardViewModel : ReactiveObject
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(() =>
        {
            // This is the data we want to gather from the wizard
            int number = 0;
            string? text = "";

            var wizard = WizardBuilder
                .StartWith(() => new FirstPageViewModel())
                .Then(prev => new SecondPageViewModel(prev.Number!.Value), x => number = x.Number!.Value)
                .Then(prev => new ThirdPageViewModel(prev), x => text = x.Text)
                .FinishWith(prev => (prev.SomeProperty, number, text));

            return dialog.ShowWizard<(string SomeProperty, int number, string text)>(wizard, "Such a nice wizard this is!");
        });

        LaunchWizard.Values()
            .SelectMany(x => Observable.FromAsync(() => dialog.ShowMessage("Wizard finished", $"This is the data we gathered from it: '{x}'")))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Maybe<(string SomeProperty, int number, string text)>> LaunchWizard { get; set; }
}