using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.ControlsNew.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace TestApp.Samples.ControlsNew.SlimWizard;

public class WizardViewModel
{
    public WizardViewModel(IDialog dialog, INotificationService notification)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            var wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), model => model.DoSomething.Enhance("Next"))
                .Then(prev => new Page2ViewModel(prev!.Value), model => ReactiveCommand.Create(() => Result.Success("123"), model.IsValid).Enhance("Finish"))
                .Build();

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            await showWizard.Execute(s => notification.Show($"This is the data we gathered from it: '{s}'", "Wizard finished"));
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<string>> LaunchWizard { get; }
}