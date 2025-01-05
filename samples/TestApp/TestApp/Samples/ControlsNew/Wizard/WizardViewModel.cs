using System.Reactive;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizard.Pages;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Avalonia.Dialogs;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizard;

using FirstPageViewModel = FirstPageViewModel;
using SecondPageViewModel = SecondPageViewModel;

public class WizardViewModel : ReactiveObject
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(() =>
        {
            var wizard = WizardBuilder
                .StartWith(() => new FirstPageViewModel())
                .Then(first => new SecondPageViewModel(first.Number!.Value))
                .Then(second => new ThirdPageViewModel())
                .Build();
            
            return dialog.Show(wizard, "Such a nice wizard this is!", closeable => wizard.OptionsForCloseable(closeable));
        });
    }

    public ReactiveCommand<Unit, bool> LaunchWizard { get; set; }
}