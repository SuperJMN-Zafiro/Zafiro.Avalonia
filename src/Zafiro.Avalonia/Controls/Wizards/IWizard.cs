using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards.Builder;

namespace Zafiro.Avalonia.Controls.Wizards;

public interface IWizard
{
    IEnhancedCommand Back { get; }
    IEnhancedCommand Next { get; }
    IStep Content { get; }
    IObservable<bool> IsLastPage { get; }
    IObservable<bool> IsValid { get; }
    IObservable<bool> IsBusy { get; }
    IObservable<int> PageIndex { get; }
    int TotalPages { get; }
}

public interface IWizard<out TResult> : IWizard
{
    TResult Result { get; }
}