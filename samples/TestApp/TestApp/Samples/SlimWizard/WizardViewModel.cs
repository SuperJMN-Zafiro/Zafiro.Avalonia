using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using TestApp.Samples.SlimWizard.Pages;
using Zafiro.Avalonia.Controls.Wizards.Slim;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Wizards.Slim;
using Zafiro.Mixins;
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
    private readonly INotificationService notification;
    private CompositeDisposable disposable = new();

    public WizardViewModel(IDialog dialog, INotificationService notification, INavigator navigator)
    {
        this.notification = notification;

        ShowWizardInDialog = ReactiveCommand.CreateFromTask(() => CreateWizard().ShowInDialog(dialog, "This is a tasty wizard"));
        NavigateToWizard = ReactiveCommand.CreateFromTask(() => CreateWizard().Navigate(navigator));

        NavigateToWizard.Merge(ShowWizardInDialog)
            .SelectMany(maybe => ShowResults(maybe).ToSignal())
            .Subscribe()
            .DisposeWith(disposable);
    }

    public ReactiveCommand<Unit, Maybe<(int result, string)>> NavigateToWizard { get; set; }

    public ReactiveCommand<Unit, Maybe<(int result, string)>> ShowWizardInDialog { get; }

    public void Dispose()
    {
        NavigateToWizard.Dispose();
        ShowWizardInDialog.Dispose();
    }

    private Task ShowResults(Maybe<(int result, string)> maybe)
    {
        var message = maybe.Match(value => $"This is the data we gathered from it: '{value}'", () => "We got nothing, because the wizard was cancelled");
        return notification.Show(message, "Finished");
    }

    private static SlimWizard<(int result, string)> CreateWizard()
    {
        return WizardBuilder
            .StartWith(() => new Page1ViewModel(), "Page 1").NextWith(model => model.ReturnSomeInt.Enhance("Next"))
            .Then(number => new Page2ViewModel(number), "Page 2").NextWhenValid((vm, number) => Result.Success((result: number, vm.Text!)))
            .Then(_ => new Page3ViewModel(), "Completed!").NextWhenValid((_, val) => Result.Success(val), "Close")
            .WithCompletionFinalStep();
    }
}