using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class WizardDesign : IWizard
{
    public int CurrentPageIndex { get; set; } = 1;
    public int TotalPages { get; } = 3;
    public object CurrentPage { get; set; } = new TextBlock() { Text = "Hello world!" };
    public MaybeViewModel<string> CurrentTitle { get; set; } = new("This is the Title of the Current Content");
    public IEnhancedUnitCommand BackCommand { get; } = EnhancedCommand.Create(ReactiveCommand.Create(() => { }));
    public IEnhancedCommand<Result<object>> NextCommand { get; } = EnhancedCommand.Create(ReactiveCommand.Create(() => Result.Success<object>(123)));
    public string NextText { get; } = "Next";
    public IObservable<object?> Finished => Finished.Cast<object>();
}