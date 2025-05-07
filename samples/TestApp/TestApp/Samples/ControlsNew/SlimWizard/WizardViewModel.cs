using System.Reactive;
using System.Threading.Tasks;
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
        LaunchWizard = ReactiveCommand.CreateFromTask(async () =>
        {
            var wizard = WizardBuilder
                .StartWith(() => new Page1ViewModel(), async model =>
                {
                    await Task.Delay(2000);
                    return Result.Success<int?>(1234);
                }, model => model.IsValid, "Continue")
                .FinishWith(number => new Page2ViewModel(number!.Value), _ => Task.FromResult(Result.Success(12)), model => model.IsValid, "Finish!");

            var showWizard = await dialog.ShowWizard(wizard, "This is a tasty wizard");
            return showWizard;
        });
    }

    public ReactiveCommand<Unit, Maybe<int>> LaunchWizard { get; }
}