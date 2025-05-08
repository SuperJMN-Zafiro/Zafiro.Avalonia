using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.ControlsNew.SlimWizard.Pages;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizard;

namespace TestApp.Samples.ControlsNew.SlimWizard;

public class WizardViewModel
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            Wizard<string> wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), model => model.DoSomething)
                .Then(prev => new Page2ViewModel(prev!.Value), model => { return EnhancedCommand.Create(ReactiveCommand.Create<Unit, Result<string>>(_ => Result.Success("123"))); })
                .Build();

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<string>> LaunchWizard { get; }
}