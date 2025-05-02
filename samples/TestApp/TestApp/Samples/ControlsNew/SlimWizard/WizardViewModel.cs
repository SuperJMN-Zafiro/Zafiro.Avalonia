using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.ControlsNew.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI.Wizard;

namespace TestApp.Samples.ControlsNew.SlimWizard;

public class WizardViewModel
{
    public WizardViewModel(IDialog dialog)
    {
        var wizard = WizardBuilder
            .StartWith(() => new Page1ViewModel(), model => model.Number, model => model.IsValid)
            .FinishWith(number => new Page2ViewModel(number!.Value), _ => Unit.Default, model => model.IsValid);

        this.Wizard = wizard;

        LaunchWizard = ReactiveCommand.CreateFromTask(() => dialog.ShowWizard(wizard, "Such a nice wizard this is!"));
    }

    public ReactiveCommand<Unit, Maybe<Unit>> LaunchWizard { get; }

    public Wizard<Unit> Wizard { get; }
}