using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Controls.Wizards;

public interface IWizard
{
    public IEnhancedCommand Back { get; }
    public IEnhancedCommand Next { get; }
    public IValidatable Content { get; }
    IObservable<bool> IsLastPage { get; }
}