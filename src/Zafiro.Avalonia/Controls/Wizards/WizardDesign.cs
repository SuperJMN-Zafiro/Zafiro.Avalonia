using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls.Wizards;

public class WizardDesign : IWizard
{
    public IEnhancedCommand Back { get; } = EnhancedCommand.Enhance(ReactiveCommand.Create(() => { }));
    public IEnhancedCommand Next { get; } = EnhancedCommand.Enhance(ReactiveCommand.Create(() => { }));
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