using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace Zafiro.Avalonia.Controls.Wizards;

public interface IWizard
{
    public IEnhancedCommand Back { get; }
    public IEnhancedCommand Next { get; }
    public IStep Content { get; }
    IObservable<bool> IsLastPage { get; }
    IObservable<bool> IsValid { get; }
    IObservable<bool> IsBusy { get; }
}