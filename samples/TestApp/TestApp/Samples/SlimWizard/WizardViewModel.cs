using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Wizards.Slim;
using Zafiro.UI;
using Zafiro.UI.Commands;
using Zafiro.UI.Shell.Utils;
using Zafiro.UI.Wizards.Slim.Builder;

namespace TestApp.Samples.SlimWizard;

[Section("Wizard", "mdi-wizard-hat", 1)]
public class WizardViewModel
{
    public WizardViewModel(IDialog dialog, INotificationService notification)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            var number = 0;
            var text = "";

            var wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), "Page 1").ProceedWith(model => model.ReturnSomeInt.Enhance())
                .Then(prev => new Page2ViewModel(prev!.Value), "Page 2").ProceedWithResultWhenValid(model => Result.Success((model.Number, model.Text)), "Peito")
                .Then(_ => new Page3ViewModel(), "Completed!").ProceedWithResultWhenValid(_ => Result.Success((number, text)))
                .WithCompletionFinalStep();

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            await showWizard.Execute(gatheredData => notification.Show($"This is the data we gathered from it: '{gatheredData}'", "Wizard finished"));
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<(int number, string text)>> LaunchWizard { get; }
}