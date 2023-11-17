using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Model;
using Zafiro.Avalonia.Wizard;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel
{
    private readonly IDialogService dialogService;

    public WizardSampleViewModel(IDialogService dialogService, INotificationService notificationService)
    {
        this.dialogService = dialogService;
        ShowWizard = ReactiveCommand.CreateFromTask(OnShowWizard);
        ShowWizard
            .Values()
            .SelectMany(s => Observable.FromAsync(() => notificationService.Show($"Result: {s}", "Wizard success!")))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Maybe<string>> ShowWizard { get; }

    private Task<Maybe<string>> OnShowWizard()
    {
        var wizard = new Wizard<Page1ViewModel, Page2ViewModel, string>(
            new Page<Page1ViewModel>(new Page1ViewModel(), "Next", "Page 1"),
            new Page<Page2ViewModel>(new Page2ViewModel(), "Finish", "Page 2"),
            (page1, page2) => string.Join(",", new[] { page1.Message, page2.Message }));

        return dialogService.ShowDialog<Wizard<Page1ViewModel, Page2ViewModel, string>, string>(wizard, "Do something, boi");
    }
}

internal class Page2ViewModel : ReactiveValidationObject, IValidatable
{
    public Page2ViewModel()
    {
        this.ValidationRule(x => x.Message, x => !string.IsNullOrWhiteSpace(x), "Can't be blank");
    }

    public IObservable<bool> IsValid => this.IsValid();
    [Reactive]
    public string? Message { get; set; }
}

internal class Page1ViewModel : ReactiveValidationObject, IValidatable
{
    public Page1ViewModel()
    {
        this.ValidationRule(x => x.Message, x => !string.IsNullOrWhiteSpace(x), "Can't be blank");
    }

    public IObservable<bool> IsValid => this.IsValid();
    [Reactive]
    public string? Message { get; set; }
}