using System.Reactive;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizards.Pages;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizards.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizards.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizards;

using FirstPageViewModel = Pages.FirstPageViewModel;
using SecondPageViewModel = Pages.SecondPageViewModel;

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
            
            return dialog.Show(wizard, "Welcome to Avalonia!", closeable =>
            [
                OptionBuilder.Create("Back", wizard.Back),
                OptionBuilder.Create("Next", wizard.Next),
                OptionBuilder.Create("Close", EnhancedCommand.Create(ReactiveCommand.Create(closeable.Close, wizard.IsLastPage)))
            ]);
        });
    }

    public ReactiveCommand<Unit, bool> LaunchWizard { get; set; }
}