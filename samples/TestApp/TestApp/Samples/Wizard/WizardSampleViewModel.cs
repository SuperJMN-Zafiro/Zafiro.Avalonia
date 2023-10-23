using System;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Model;
using Zafiro.Avalonia.Wizard;

namespace TestApp.Samples.Wizard;

public class WizardSampleViewModel
{
    private readonly IDialogService dialogService;

    public WizardSampleViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
        ShowWizard = ReactiveCommand.CreateFromTask(OnShowWizard);
    }

    public ReactiveCommand<Unit, Maybe<string>> ShowWizard { get; }

    private Task<Maybe<string>> OnShowWizard()
    {
        var wizard = new Wizard<Page1, Page2, string>(
            new Page<Page1>(new Page1(), "Text", "Page 1"),
            new Page<Page2>(new Page2(), "Text", "Page 1"),
            (page1, page2) => string.Join(page1.Message, page2.Message));

        return dialogService.ShowDialog<Wizard<Page1, Page2, string>, string>(wizard, "Do something, boi");
    }
}

internal class Page2 : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid => this.IsValid();
    public IEnumerable<string> Message { get; set; }
}

internal class Page1 : ReactiveValidationObject, IValidatable
{
    public IObservable<bool> IsValid => this.IsValid();
    public string? Message { get; set; }
}