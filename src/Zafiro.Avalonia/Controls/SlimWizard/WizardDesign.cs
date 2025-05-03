using System.Reactive;
using Zafiro.UI.Wizard;

namespace Zafiro.Avalonia.Controls.SlimWizard;

public class WizardDesign : IWizard
{
    public int CurrentIndex { get; set; } = 1;
    public object CurrentPage { get; set; } = new TextBlock() { Text = "Hello world!" };
    public MaybeViewModel<string> CurrentTitle { get; set; } = new("This is the Title of the Current Content");
    public ReactiveCommand<Unit, Unit> BackCommand { get; } = ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, Unit> NextCommand { get; } = ReactiveCommand.Create(() => { });
}