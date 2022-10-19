using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia;

namespace MultiSelectorSample.ViewModels;

public class ViewModel : ViewModelBase, ISelectable
{
    [Reactive]
    public bool IsSelected { get; set; }
}