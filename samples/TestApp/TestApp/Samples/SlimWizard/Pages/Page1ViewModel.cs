using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI.Commands;

namespace TestApp.Samples.SlimWizard.Pages;

public partial class Page1ViewModel : ReactiveValidationObject
{
    [Reactive] private int? number;

    public Page1ViewModel()
    {
        this.ValidationRule<Page1ViewModel, int?>(x => x.Number, i => i % 2 == 0, "Number must be even");
        ReturnSomeInt = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Delay(1000);
            return Result.Success<int?>(Number);
        }, this.IsValid()).Enhance();
    }

    public IEnhancedCommand<Result<int?>> ReturnSomeInt { get; set; }

    public IObservable<bool> IsValid => this.IsValid();
    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
}