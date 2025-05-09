using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls.Wizards;

public class WizardDesign : IWizard
{
    public IEnhancedCommand Back { get; } = ReactiveCommand.Create(() => { }).Enhance();
    public IEnhancedCommand Next { get; } = ReactiveCommand.Create(() => { }).Enhance();
    public IStep Content { get; } = new StepDesign();
    public IObservable<bool> IsLastPage => Observable.Return(false);
    public IObservable<bool> IsValid => Observable.Return(true);
    public IObservable<bool> IsBusy => Observable.Return(false);
    public IObservable<int> PageIndex => Observable.Return(2);
    public int TotalPages => 5;
}

public class StepDesign : IStep
{
    public IObservable<bool> IsValid => Observable.Return(true);
    public IObservable<bool> IsBusy => Observable.Return(false);
    public bool AutoAdvance => false;
    public Maybe<string> Title => "Sample step with a very long title";
}