using System.Reactive;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizard.Pages;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizard;

public class WizardViewModel : ReactiveObject
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(() =>
        {
            var pages = WizardBuilder
                .StartWith(() => new FirstPageViewModel())
                .Then(first => new SecondPageViewModel(first.Number!.Value))
                .Then(second => new ThirdPageViewModel())
                .Build();
            
            var wizard = new Zafiro.Avalonia.Controls.Wizards.Wizard(pages);

            return dialog.Show(wizard, "Welcome to Avalonia!", closeable =>
            [
                OptionBuilder.Create("Back", wizard.Back),
                OptionBuilder.Create("Next", wizard.Next),
                OptionBuilder.Create("Close", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close)))
            ]);
        });
    }

    public ReactiveCommand<Unit, bool> LaunchWizard { get; set; }
}