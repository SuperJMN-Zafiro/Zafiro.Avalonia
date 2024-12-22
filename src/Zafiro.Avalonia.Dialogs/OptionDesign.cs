using ReactiveUI;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public class OptionDesign : IOption
{
    public string Title { get; set; }
    public IEnhancedCommand Command { get; } = EnhancedCommand.Create(ReactiveCommand.Create(() => { }));
    public bool IsDefault { get; set; }
    public bool IsCancel { get; set; }
    public IObservable<bool> IsVisible { get; set; }
}