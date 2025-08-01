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
            var wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), "Page 1").ProceedWith(model => model.ReturnSomeInt.Enhance())
                .Then(number => new Page2ViewModel(number), "Page 2").ProceedWithResultWhenValid((vm, number) => Result.Success((result: number, vm.Text!)))
                .Then(_ => new Page3ViewModel(), "Completed!").ProceedWithResultWhenValid((_, val) => Result.Success(val))
                .WithCompletionFinalStep();

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            await showWizard.Execute(gatheredData => notification.Show($"This is the data we gathered from it: '{gatheredData}'", "Wizard finished"));
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<(int result, string)>> LaunchWizard { get; }
}