using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;
using Zafiro.UI.Shell.Utils;
using Zafiro.UI.Wizards.Slim;
using Zafiro.UI.Wizards.Slim.Builder;

namespace TestApp.Samples.SlimWizard;

[Section("Wizard", "mdi-wizard-hat", 1)]
public class WizardViewModel : IDisposable
{
    public WizardViewModel(IDialog dialog, INotificationService notification, INavigator navigator)
    {
        LaunchWizardDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var wizard = CreateWizard();

            var result = await wizard.ShowDialog(dialog, "This is a tasty wizard");
            await result.Execute(gatheredData => notification.Show($"This is the data we gathered from it: '{gatheredData}'", "Wizard finished"));
            return result;
        });

        StartWizard = ReactiveCommand.CreateFromTask(() => CreateWizard().Navigate(navigator));
        StartWizard.Subscribe(maybe => { });
    }

    public ReactiveCommand<Unit, Maybe<(int result, string)>> StartWizard { get; set; }

    public ISlimWizard Wizard { get; set; }

    public ReactiveCommand<Unit, Maybe<(int result, string)>> LaunchWizardDialog { get; }

    public void Dispose()
    {
        StartWizard.Dispose();
        LaunchWizardDialog.Dispose();
    }

    private static SlimWizard<(int result, string)> CreateWizard()
    {
        return WizardBuilder
            .StartWith(() => new Page1ViewModel(), "Page 1").ProceedWith(model => model.ReturnSomeInt.Enhance())
            .Then(number => new Page2ViewModel(number), "Page 2").ProceedWithResultWhenValid((vm, number) => Result.Success((result: number, vm.Text!)))
            .Then(_ => new Page3ViewModel(), "Completed!").ProceedWithResultWhenValid((_, val) => Result.Success(val), "Close")
            .WithCompletionFinalStep();
    }
}