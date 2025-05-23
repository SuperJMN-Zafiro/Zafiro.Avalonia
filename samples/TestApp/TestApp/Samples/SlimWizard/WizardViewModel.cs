using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.Adorners;
using TestApp.Samples.ControlsNew.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Wizards.Slim;
using Zafiro.UI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim.Builder;

namespace TestApp.Samples.ControlsNew.SlimWizard;

[Icon("mdi-wizard-hat")]
public class WizardViewModel
{
    public WizardViewModel(IDialog dialog, INotificationService notification)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            var number = 0;
            var text = "";

            var wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), model => model.DoSomething.Enhance("Next"), "Page 1")
                .Then(prev => new Page2ViewModel(prev!.Value), model => ReactiveCommand.Create(() =>
                {
                    number = model.Number;
                    text = model.Text;
                    return Result.Success(Unit.Default);
                }, model.IsValid).Enhance("Next"), "Page 2")
                .Then(_ => new Page3ViewModel(), model => ReactiveCommand.Create(() => Result.Success((number, text)), model.IsValid).Enhance("Close"), "Completed!")
                .WithCompletionFinalStep();

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            await showWizard.Execute(gatheredData => notification.Show($"This is the data we gathered from it: '{gatheredData}'", "Wizard finished"));
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<(int number, string text)>> LaunchWizard { get; }
}