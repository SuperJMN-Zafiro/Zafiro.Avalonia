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
            .FinishWith(number => new Page2ViewModel(number!.Value), _ => 12, model => model.IsValid);

        this.Wizard = wizard;

        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            var showWizard = await dialog.ShowWizard(wizard, "Such a nice wizard this is!");
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<int>> LaunchWizard { get; }

    public Wizard<int> Wizard { get; }
}