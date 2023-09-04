namespace Zafiro.Avalonia.Controls.Wizard;

public interface IWizardPage
{
    IObservable<bool> IsValid { get; }
    public string NextText { get; }
}