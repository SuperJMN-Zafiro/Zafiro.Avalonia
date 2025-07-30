using Zafiro.Avalonia.Controls.Wizards.Slim;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Dialogs.Wizards.Slim;

public interface ISlimWizardDialogHost : IDialogHeaderProvider
{
    ISlimWizard Wizard { get; }
}

public class SlimWizardDialogHost<TResult> : ISlimWizard<TResult>, ISlimWizardDialogHost
{
    public SlimWizardDialogHost(ISlimWizard<TResult> wizard)
    {
        Wizard = wizard;
        Header = new SlimWizardHeader { Wizard = wizard };
    }

    public ISlimWizard<TResult> Wizard { get; }
    ISlimWizard ISlimWizardDialogHost.Wizard => Wizard;
    public object Header { get; }

    public IEnhancedCommand Next => Wizard.Next;
    public IEnhancedCommand Back => Wizard.Back;
    public IPage CurrentPage => Wizard.CurrentPage;
    public int TotalPages => Wizard.TotalPages;
    public IObservable<TResult> Finished => Wizard.Finished;
}
