using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Model;

public interface IWizardPage
{
    IObservable<bool> IsValid { get; }
    public Maybe<string> NextText { get; }
}