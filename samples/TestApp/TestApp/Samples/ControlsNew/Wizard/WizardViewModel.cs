using System;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using TestApp.Samples.ControlsNew.Wizard.Pages;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Navigation;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.UI;
using FirstPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.FirstPageViewModel;
using SecondPageViewModel = TestApp.Samples.ControlsNew.Wizard.Pages.SecondPageViewModel;

namespace TestApp.Samples.ControlsNew.Wizard;

public class WizardViewModel : ReactiveObject
{
    public WizardViewModel(IDialog dialog)
    {
        LaunchWizard = ReactiveCommand.CreateFromTask(() =>
        {
            IList<Func<IValidatable>> pages =
            [
                () => new FirstPageViewModel(),
                () => new SecondPageViewModel(),
                () => new ThirdPageViewModel()
            ];
            
            var wizard = new Zafiro.Avalonia.Controls.Wizards.Wizard(pages, new Navigator());
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